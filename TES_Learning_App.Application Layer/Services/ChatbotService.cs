using System.Text;
using System.Linq;
using System.Text.Json;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TES_Learning_App.Application_Layer.DTOs.Chatbot;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.Application_Layer.Services
{
    public class ChatbotService : IChatbotService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ChatbotService> _logger;
        private readonly string _apiKey;
        private readonly string _knowledgeBasePath;

        public ChatbotService(IConfiguration configuration, ILogger<ChatbotService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _apiKey = _configuration["GoogleAI:ApiKey"] ?? string.Empty;
            
            // Log API key status (masked for security)
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                _logger.LogWarning("Google AI API key is not configured in appsettings.json");
            }
            else
            {
                var maskedKey = _apiKey.Length > 8 
                    ? $"{_apiKey.Substring(0, 4)}...{_apiKey.Substring(_apiKey.Length - 4)}" 
                    : "****";
                _logger.LogInformation("Google AI API key loaded: {MaskedKey}", maskedKey);
            }
            
            // Get knowledge base path - try multiple locations
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var possiblePaths = new[]
            {
                Path.Combine(baseDirectory, "KnowledgeBase"),
                Path.Combine(Directory.GetParent(baseDirectory)?.FullName ?? "", "KnowledgeBase"),
                Path.Combine(baseDirectory, "..", "KnowledgeBase"),
                Path.Combine(Directory.GetCurrentDirectory(), "KnowledgeBase")
            };

            _knowledgeBasePath = possiblePaths.FirstOrDefault(Directory.Exists) ?? possiblePaths[0];

            // Ensure knowledge base directory exists
            if (!Directory.Exists(_knowledgeBasePath))
            {
                try
                {
                    Directory.CreateDirectory(_knowledgeBasePath);
                    _logger.LogInformation("Created knowledge base directory: {Path}", _knowledgeBasePath);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not create knowledge base directory: {Path}", _knowledgeBasePath);
                }
            }
            else
            {
                _logger.LogInformation("Using knowledge base directory: {Path}", _knowledgeBasePath);
            }
        }

        public async Task<ChatbotResponse> GetResponseAsync(ChatbotRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_apiKey))
                {
                    _logger.LogWarning("Google AI API key is not configured");
                    return new ChatbotResponse
                    {
                        IsSuccess = false,
                        Error = "AI service is not configured. Please add your Google AI API key to appsettings.json under 'GoogleAI:ApiKey'. Get your API key from: https://makersuite.google.com/app/apikey"
                    };
                }

                // Validate API key format
                if (!_apiKey.StartsWith("AIza"))
                {
                    _logger.LogWarning("API key format appears incorrect. Expected format: AIza...");
                    return new ChatbotResponse
                    {
                        IsSuccess = false,
                        Error = "API key format is incorrect. Please verify your Google AI API key in appsettings.json. It should start with 'AIza'."
                    };
                }

                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    return new ChatbotResponse
                    {
                        IsSuccess = false,
                        Error = "Message cannot be empty"
                    };
                }

                // Load knowledge base
                var knowledgeBase = await LoadKnowledgeBaseAsync();

                // Build prompt with knowledge base context
                var systemPrompt = $@"You are a helpful AI assistant for the Trilingo Admin Panel. 
You help administrators manage the learning platform, answer questions about the system, and provide guidance.

Knowledge Base:
{knowledgeBase}

Instructions:
- Answer questions based on the knowledge base above
- Be concise and helpful
- If you don't know something, say so
- Use a friendly, professional tone
- Focus on helping with admin panel operations";

                var fullPrompt = $"{systemPrompt}\n\nUser Question: {request.Message}\n\nAssistant Response:";

                // Call Google Generative AI (Gemini)
                var response = await CallGoogleAIAsync(fullPrompt);

                return new ChatbotResponse
                {
                    IsSuccess = true,
                    Message = response,
                    ConversationId = request.ConversationId ?? Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ChatbotService.GetResponseAsync: {Message}", ex.Message);
                return new ChatbotResponse
                {
                    IsSuccess = false,
                    Error = $"I apologize, but I encountered an error: {ex.Message}. Please try again."
                };
            }
        }

        private async Task<List<string>?> GetAvailableModelsAsync(HttpClient httpClient)
        {
            try
            {
                var listUrl = $"https://generativelanguage.googleapis.com/v1beta/models?key={_apiKey}";
                var listResponse = await httpClient.GetAsync(listUrl);
                
                if (listResponse.IsSuccessStatusCode)
                {
                    var listJson = await listResponse.Content.ReadAsStringAsync();
                    var listObj = JsonSerializer.Deserialize<JsonElement>(listJson);
                    
                    var models = new List<string>();
                    if (listObj.TryGetProperty("models", out var modelsArray))
                    {
                        foreach (var model in modelsArray.EnumerateArray())
                        {
                            if (model.TryGetProperty("name", out var name))
                            {
                                var modelName = name.GetString();
                                if (!string.IsNullOrEmpty(modelName))
                                {
                                    // Extract just the model name (remove "models/" prefix if present)
                                    var cleanName = modelName.Replace("models/", "");
                                    models.Add(cleanName);
                                }
                            }
                        }
                    }
                    
                    if (models.Count > 0)
                    {
                        _logger.LogInformation("Found {Count} available models: {Models}", models.Count, string.Join(", ", models));
                        return models;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not retrieve available models list");
            }
            
            return null;
        }

        private async Task<string> CallGoogleAIAsync(string prompt)
        {
            try
            {
                // Validate API key format
                if (string.IsNullOrWhiteSpace(_apiKey))
                {
                    throw new Exception("Google AI API key is not configured. Please add it to appsettings.json under GoogleAI:ApiKey");
                }

                if (!_apiKey.StartsWith("AIza"))
                {
                    _logger.LogWarning("API key format may be incorrect. Expected format: AIza...");
                }

                // Using Google Generative AI REST API (Gemini)
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(30);
                
                // Add default headers
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Trilingo-Admin-Panel/1.0");

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        topK = 40,
                        topP = 0.95,
                        maxOutputTokens = 2048
                    },
                    safetySettings = new[]
                    {
                        new { category = "HARM_CATEGORY_HARASSMENT", threshold = "BLOCK_MEDIUM_AND_ABOVE" },
                        new { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "BLOCK_MEDIUM_AND_ABOVE" },
                        new { category = "HARM_CATEGORY_SEXUALLY_EXPLICIT", threshold = "BLOCK_MEDIUM_AND_ABOVE" },
                        new { category = "HARM_CATEGORY_DANGEROUS_CONTENT", threshold = "BLOCK_MEDIUM_AND_ABOVE" }
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

                // First, try to get available models to see what's accessible
                var availableModels = await GetAvailableModelsAsync(httpClient);
                
                // Use only v1beta (v1 doesn't support these models)
                // Try models in order of preference - newer models first
                // If we got available models, prioritize those
                var modelCandidates = new List<(string version, string model)>();
                
                if (availableModels != null && availableModels.Count > 0)
                {
                    // Use models that are actually available
                    foreach (var model in availableModels)
                    {
                        if (model.Contains("gemini"))
                        {
                            modelCandidates.Add(("v1beta", model));
                        }
                    }
                }
                
                // Fallback to standard models if we couldn't get the list
                if (modelCandidates.Count == 0)
                {
                    modelCandidates.AddRange(new[]
                    {
                        ("v1beta", "gemini-1.5-flash"),
                        ("v1beta", "gemini-1.5-pro"),
                        ("v1beta", "gemini-pro"),
                        ("v1beta", "models/gemini-1.5-flash"),  // Try with models/ prefix
                        ("v1beta", "models/gemini-1.5-pro"),
                        ("v1beta", "models/gemini-pro"),
                    });
                }
                
                var endpoints = modelCandidates.ToArray();

                HttpResponseMessage? response = null;
                string? lastError = null;
                string? successfulEndpoint = null;

                foreach (var (version, model) in endpoints)
                {
                    try
                    {
                        var url = $"https://generativelanguage.googleapis.com/{version}/models/{model}:generateContent?key={_apiKey}";
                        _logger.LogInformation("Trying Google AI API: {Version}/{Model}", version, model);
                        
                        response = await httpClient.PostAsync(url, requestContent);
                        
                        if (response.IsSuccessStatusCode)
                        {
                            successfulEndpoint = $"{version}/{model}";
                            _logger.LogInformation("Successfully connected to Google AI API: {Endpoint}", successfulEndpoint);
                            break; // Success, use this response
                        }
                        
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogWarning("Google AI API error for {Version}/{Model}: Status {StatusCode}, Content: {Content}", 
                            version, model, response.StatusCode, errorContent);
                        
                        // Parse error message
                        string parsedError = $"Status: {response.StatusCode}";
                        try
                        {
                            var errorObj = JsonSerializer.Deserialize<JsonElement>(errorContent);
                            if (errorObj.TryGetProperty("error", out var errorProp))
                            {
                                if (errorProp.TryGetProperty("message", out var errorMsg))
                                {
                                    parsedError = errorMsg.GetString() ?? parsedError;
                                }
                                else if (errorProp.TryGetProperty("status", out var status))
                                {
                                    parsedError = $"Status: {status.GetString()}";
                                }
                            }
                        }
                        catch { }
                        
                        lastError = parsedError;
                        
                        // If it's a NotFound or Forbidden, try next endpoint
                        if (response.StatusCode == System.Net.HttpStatusCode.NotFound || 
                            response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                        {
                            continue;
                        }
                        
                        // For authentication errors, don't try other endpoints
                        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            throw new Exception($"API key is invalid or expired. Please verify your Google AI API key.");
                        }
                        
                        // Continue to next endpoint for other errors
                        continue;
                    }
                    catch (Exception ex) when (!(ex.Message.Contains("API key")))
                    {
                        lastError = ex.Message;
                        _logger.LogWarning(ex, "Exception trying {Version}/{Model}", version, model);
                        continue; // Try next endpoint
                    }
                }

                if (response == null || !response.IsSuccessStatusCode)
                {
                    // Provide detailed error message with troubleshooting steps
                    var errorMsg = new StringBuilder();
                    errorMsg.Append("Failed to connect to Google AI API. ");
                    errorMsg.Append($"Tried {endpoints.Length} different models (all using v1beta). ");
                    
                    if (lastError != null)
                    {
                        errorMsg.Append($"Last error: {lastError}. ");
                    }
                    
                    errorMsg.Append("Please verify: 1) Your API key is correct in appsettings.json (GoogleAI:ApiKey), ");
                    errorMsg.Append("2) The Generative Language API is enabled in Google Cloud Console, ");
                    errorMsg.Append("3) Your API key has access to Gemini models, ");
                    errorMsg.Append("4) Billing is enabled for your Google Cloud project (required for Gemini API). ");
                    errorMsg.Append("Get a new API key from: https://makersuite.google.com/app/apikey");
                    
                    _logger.LogError("Google AI API connection failed. {ErrorDetails}", errorMsg.ToString());
                    throw new Exception(errorMsg.ToString());
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                var responseObj = JsonSerializer.Deserialize<JsonElement>(responseJson);

                // Extract the generated text
                if (responseObj.TryGetProperty("candidates", out var candidates) &&
                    candidates.GetArrayLength() > 0)
                {
                    var firstCandidate = candidates[0];
                    if (firstCandidate.TryGetProperty("content", out var contentObj) &&
                        contentObj.TryGetProperty("parts", out var parts) &&
                        parts.GetArrayLength() > 0)
                    {
                        var firstPart = parts[0];
                        if (firstPart.TryGetProperty("text", out var text))
                        {
                            return text.GetString() ?? "I apologize, but I couldn't generate a response.";
                        }
                    }
                }

                return "I apologize, but I couldn't generate a response.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Google AI API: {Message}", ex.Message);
                // Return a user-friendly error message instead of throwing
                return $"I apologize, but I'm having trouble connecting to the AI service. Error: {ex.Message}. Please try again later.";
            }
        }

        public async Task<string> LoadKnowledgeBaseAsync()
        {
            try
            {
                var knowledgeBaseBuilder = new StringBuilder();

                if (!Directory.Exists(_knowledgeBasePath))
                {
                    return "No knowledge base available.";
                }

                var mdFiles = Directory.GetFiles(_knowledgeBasePath, "*.md", SearchOption.AllDirectories);

                foreach (var file in mdFiles)
                {
                    try
                    {
                        var content = await File.ReadAllTextAsync(file);
                        var fileName = Path.GetFileName(file);
                        knowledgeBaseBuilder.AppendLine($"\n--- {fileName} ---\n");
                        knowledgeBaseBuilder.AppendLine(content);
                        knowledgeBaseBuilder.AppendLine("\n");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error reading knowledge base file: {File}", file);
                    }
                }

                return knowledgeBaseBuilder.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading knowledge base");
                return "Error loading knowledge base.";
            }
        }
    }
}

