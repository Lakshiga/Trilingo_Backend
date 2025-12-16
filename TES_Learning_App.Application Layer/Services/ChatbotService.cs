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

                // Note: We don't validate API key format here - let the actual API call validate it
                // This allows for different API key formats and better error messages from Google's API

                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    return new ChatbotResponse
                    {
                        IsSuccess = false,
                        Error = "Message cannot be empty"
                    };
                }

                // Check if user wants to generate an image
                var messageLower = request.Message.ToLowerInvariant();
                var imageKeywords = new[] { "generate image", "create image", "draw", "make an image", "show me an image", "image of", "picture of", "generate a picture", "create a picture" };
                var isImageRequest = imageKeywords.Any(keyword => messageLower.Contains(keyword));

                if (isImageRequest)
                {
                    // Extract the image description from the request
                    var imageDescription = ExtractImageDescription(request.Message);
                    
                    // Generate image using Gemini image generation
                    var imageResult = await GenerateImageAsync(imageDescription);
                    
                    if (!string.IsNullOrEmpty(imageResult.ImageData))
                    {
                        return new ChatbotResponse
                        {
                            IsSuccess = true,
                            Message = imageResult.Message ?? "Here's the image you requested:",
                            ImageData = imageResult.ImageData,
                            HasImage = true,
                            ConversationId = request.ConversationId ?? Guid.NewGuid().ToString()
                        };
                    }
                    else
                    {
                        // If image generation failed, try to provide a detailed description as fallback
                        if (!string.IsNullOrEmpty(imageResult.Error))
                        {
                            // Try to get a concise description from Gemini as a helpful alternative
                            var descriptionResult = await GenerateImageDescriptionAsync(imageDescription);
                            
                            return new ChatbotResponse
                            {
                                IsSuccess = true,
                                Message = $"I couldn't generate an image, but here's what it would look like:\n\n{descriptionResult}",
                                ConversationId = request.ConversationId ?? Guid.NewGuid().ToString()
                            };
                        }
                        
                        return new ChatbotResponse
                        {
                            IsSuccess = false,
                            Error = imageResult.Error ?? "I apologize, but I couldn't generate the image. Please try again with a different description."
                        };
                    }
                }

                // Load knowledge base
                var knowledgeBase = await LoadKnowledgeBaseAsync();

                // Build prompt with knowledge base context
                var systemPrompt = $@"You are a helpful AI assistant for the Trilingo Admin Panel. 
You help administrators manage the learning platform, answer questions about the system, and provide guidance.
You can also generate images when users request them using phrases like 'generate image', 'create image', 'draw', etc.

Knowledge Base:
{knowledgeBase}

Instructions:
- Answer questions based on the knowledge base above
- Be concise and helpful
- If you don't know something, say so
- Use a friendly, professional tone
- Focus on helping with admin panel operations
- You can generate images when requested";

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
                                    // Skip embedding models - they don't support generateContent
                                    if (modelName.Contains("embedding", StringComparison.OrdinalIgnoreCase))
                                    {
                                        continue;
                                    }
                                    
                                    // Check if model supports generateContent method
                                    bool supportsGenerateContent = false;
                                    if (model.TryGetProperty("supportedGenerationMethods", out var methods))
                                    {
                                        foreach (var method in methods.EnumerateArray())
                                        {
                                            var methodStr = method.GetString();
                                            if (methodStr == "generateContent")
                                            {
                                                supportsGenerateContent = true;
                                                break;
                                            }
                                        }
                                    }
                                    
                                    // Only add models that support generateContent or are known Gemini models
                                    if (supportsGenerateContent || modelName.Contains("gemini", StringComparison.OrdinalIgnoreCase))
                                    {
                                        // Extract just the model name (remove "models/" prefix if present)
                                        var cleanName = modelName.Replace("models/", "");
                                        models.Add(cleanName);
                                    }
                                }
                            }
                        }
                    }
                    
                    if (models.Count > 0)
                    {
                        _logger.LogInformation("Found {Count} available generative models: {Models}", models.Count, string.Join(", ", models));
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
                    // Use models that are actually available and filter out embedding models
                    foreach (var model in availableModels)
                    {
                        // Skip embedding models explicitly
                        if (model.Contains("embedding", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        
                        // Only add Gemini generative models
                        if (model.Contains("gemini", StringComparison.OrdinalIgnoreCase))
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

        private string ExtractImageDescription(string message)
        {
            // Remove common image generation phrases to extract the actual description
            var description = message;
            var phrasesToRemove = new[]
            {
                "generate image of", "generate image", "create image of", "create image",
                "draw", "make an image of", "make an image", "show me an image of",
                "show me an image", "image of", "picture of", "generate a picture of",
                "generate a picture", "create a picture of", "create a picture"
            };

            foreach (var phrase in phrasesToRemove)
            {
                var index = description.IndexOf(phrase, StringComparison.OrdinalIgnoreCase);
                if (index >= 0)
                {
                    description = description.Substring(index + phrase.Length).Trim();
                    break;
                }
            }

            // If no phrase was found, return the original message
            return string.IsNullOrWhiteSpace(description) ? message : description;
        }

        private async Task<(string? ImageData, string? Message, string? Error)> GenerateImageAsync(string description)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_apiKey))
                {
                    return (null, null, "Google AI API key is not configured.");
                }

                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(60); // Image generation may take longer
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Trilingo-Admin-Panel/1.0");

                // Try image generation models - Gemini 2.5 Flash Image Preview or Imagen models
                // Note: Image generation models may require specific API access
                var imageModels = new[]
                {
                    ("gemini-2.5-flash-image-preview", true),
                    ("gemini-2.0-flash-exp-image-generation", true),
                    ("gemini-1.5-flash", false), // Some versions might support image generation
                    ("gemini-1.5-pro", false) // Fallback: might support some image generation
                };

                foreach (var (model, isImageModel) in imageModels)
                {
                    try
                    {
                        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={_apiKey}";
                        
                        object requestBody;
                        
                        if (isImageModel)
                        {
                            // For image generation models, use a simple, direct prompt
                            // The model should automatically generate an image when requested
                            requestBody = new
                            {
                                contents = new[]
                                {
                                    new
                                    {
                                        parts = new[]
                                        {
                                            new { text = description }
                                        }
                                    }
                                },
                                generationConfig = new
                                {
                                    temperature = 0.4,
                                    maxOutputTokens = 1024,
                                    responseModalities = new[] { "IMAGE" }
                                },
                                safetySettings = new[]
                                {
                                    new { category = "HARM_CATEGORY_HARASSMENT", threshold = "BLOCK_ONLY_HIGH" },
                                    new { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "BLOCK_ONLY_HIGH" },
                                    new { category = "HARM_CATEGORY_SEXUALLY_EXPLICIT", threshold = "BLOCK_ONLY_HIGH" },
                                    new { category = "HARM_CATEGORY_DANGEROUS_CONTENT", threshold = "BLOCK_ONLY_HIGH" }
                                }
                            };
                        }
                        else
                        {
                            // For regular models, try requesting image generation
                            requestBody = new
                            {
                                contents = new[]
                                {
                                    new
                                    {
                                        parts = new[]
                                        {
                                            new { text = $"Generate a high-quality, detailed image of: {description}. Make it visually appealing and clear." }
                                        }
                                    }
                                },
                                generationConfig = new
                                {
                                    temperature = 0.7,
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
                        }

                        var json = JsonSerializer.Serialize(requestBody);
                        var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

                        _logger.LogInformation("Attempting image generation with model: {Model}, description: {Description}", model, description);
                        var response = await httpClient.PostAsync(url, requestContent);
                        var responseContent = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            _logger.LogInformation("API call successful for model {Model}. Response length: {Length}", model, responseContent.Length);
                            var responseObj = JsonSerializer.Deserialize<JsonElement>(responseContent);

                            // Log full response structure for debugging
                            _logger.LogDebug("Full API response for {Model}: {Response}", model, responseContent);

                            // Extract image data from response
                            if (responseObj.TryGetProperty("candidates", out var candidates) &&
                                candidates.GetArrayLength() > 0)
                            {
                                var firstCandidate = candidates[0];
                                
                                // Check for finish reason to see if generation was blocked
                                if (firstCandidate.TryGetProperty("finishReason", out var finishReason))
                                {
                                    var reason = finishReason.GetString();
                                    _logger.LogInformation("Finish reason for {Model}: {Reason}", model, reason);
                                    
                                    if (reason == "SAFETY" || reason == "RECITATION")
                                    {
                                        _logger.LogWarning("Image generation blocked by safety filters for model {Model}", model);
                                        continue; // Try next model
                                    }
                                }
                                
                                if (firstCandidate.TryGetProperty("content", out var contentObj) &&
                                    contentObj.TryGetProperty("parts", out var parts))
                                {
                                    string? extractedImage = null;
                                    string? textResponse = null;
                                    string? mimeType = null;
                                    
                                    foreach (var part in parts.EnumerateArray())
                                    {
                                        // Check for inline data (base64 image)
                                        if (part.TryGetProperty("inlineData", out var inlineData))
                                        {
                                            if (inlineData.TryGetProperty("data", out var imageData))
                                            {
                                                var base64Image = imageData.GetString();
                                                if (!string.IsNullOrEmpty(base64Image))
                                                {
                                                    extractedImage = base64Image;
                                                    _logger.LogInformation("Found image data in response, length: {Length}", base64Image.Length);
                                                }
                                            }
                                            
                                            if (inlineData.TryGetProperty("mimeType", out var mime))
                                            {
                                                mimeType = mime.GetString();
                                                _logger.LogInformation("Image MIME type: {MimeType}", mimeType);
                                            }
                                        }
                                        
                                        // Check for text response
                                        if (part.TryGetProperty("text", out var text))
                                        {
                                            textResponse = text.GetString();
                                            _logger.LogInformation("Text response from model {Model}: {Text}", model, textResponse);
                                        }
                                    }
                                    
                                    if (!string.IsNullOrEmpty(extractedImage))
                                    {
                                        _logger.LogInformation("Successfully generated image using model: {Model}, size: {Size} bytes", model, extractedImage.Length);
                                        return (extractedImage, textResponse ?? "Here's the image you requested:", null);
                                    }
                                    
                                    // If we got text but no image, log it and try next model
                                    if (!string.IsNullOrEmpty(textResponse))
                                    {
                                        _logger.LogWarning("Model {Model} returned text but no image. Text: {Text}", model, textResponse);
                                    }
                                    else
                                    {
                                        _logger.LogWarning("Model {Model} returned response but no image or text found in parts", model);
                                    }
                                }
                                else
                                {
                                    _logger.LogWarning("Model {Model} response missing content or parts", model);
                                }
                            }
                            else
                            {
                                _logger.LogWarning("Model {Model} response missing candidates array", model);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Image generation failed for model {Model}: Status {StatusCode}, Response: {Error}", 
                                model, response.StatusCode, responseContent);
                            
                            // Parse error message for better user feedback
                            try
                            {
                                var errorObj = JsonSerializer.Deserialize<JsonElement>(responseContent);
                                if (errorObj.TryGetProperty("error", out var errorProp))
                                {
                                    if (errorProp.TryGetProperty("message", out var errorMsg))
                                    {
                                        var errorMessage = errorMsg.GetString();
                                        _logger.LogWarning("API error message: {Message}", errorMessage);
                                        
                                        // If it's a permission/access error, don't try other models
                                        if (errorMessage != null && (
                                            errorMessage.Contains("not found", StringComparison.OrdinalIgnoreCase) ||
                                            errorMessage.Contains("permission", StringComparison.OrdinalIgnoreCase) ||
                                            errorMessage.Contains("access", StringComparison.OrdinalIgnoreCase)))
                                        {
                                            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                                            {
                                                continue; // Try next model
                                            }
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                // If error parsing fails, continue to next model
                            }
                            
                            // If it's a 404, try next model
                            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                            {
                                continue;
                            }
                            
                            // For 403/401, might be permission issue - try next model
                            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden || 
                                response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                            {
                                continue;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error trying image generation with model {Model}", model);
                        continue; // Try next model
                    }
                }

                // If all models failed, try alternative approach using text-to-image via Imagen API
                var imagenResult = await TryImagenApiAsync(description);
                if (imagenResult.ImageData != null)
                {
                    return imagenResult;
                }
                
                // If all attempts failed, return a helpful error message
                _logger.LogError("All image generation attempts failed. Check logs above for details.");
                return (null, null, "Image generation is currently unavailable. Your API key may need access to Gemini image models. " +
                    "I'll provide a detailed description instead.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GenerateImageAsync: {Message}", ex.Message);
                return (null, null, $"Failed to generate image: {ex.Message}");
            }
        }

        private async Task<(string? ImageData, string? Message, string? Error)> TryImagenApiAsync(string description)
        {
            try
            {
                _logger.LogInformation("Trying alternative image generation approach for: {Description}", description);
                
                // Try a few more model variations that might support image generation
                var alternativeModels = new[]
                {
                    "gemini-pro-vision",
                    "gemini-1.5-flash-latest",
                    "gemini-1.5-pro-latest"
                };

                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(60);
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Trilingo-Admin-Panel/1.0");

                foreach (var model in alternativeModels)
                {
                    try
                    {
                        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={_apiKey}";
                        
                        var requestBody = new
                        {
                            contents = new[]
                            {
                                new
                                {
                                    parts = new[]
                                    {
                                        new { text = description }
                                    }
                                }
                            },
                            generationConfig = new
                            {
                                temperature = 0.4,
                                maxOutputTokens = 1024
                            }
                        };

                        var json = JsonSerializer.Serialize(requestBody);
                        var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

                        _logger.LogInformation("Trying alternative model: {Model}", model);
                        var response = await httpClient.PostAsync(url, requestContent);
                        var responseContent = await response.Content.ReadAsStringAsync();
                        
                        if (response.IsSuccessStatusCode)
                        {
                            var responseObj = JsonSerializer.Deserialize<JsonElement>(responseContent);

                            // Try to extract image from response
                            if (responseObj.TryGetProperty("candidates", out var candidates) &&
                                candidates.GetArrayLength() > 0)
                            {
                                var firstCandidate = candidates[0];
                                if (firstCandidate.TryGetProperty("content", out var contentObj) &&
                                    contentObj.TryGetProperty("parts", out var parts))
                                {
                                    foreach (var part in parts.EnumerateArray())
                                    {
                                        if (part.TryGetProperty("inlineData", out var inlineData))
                                        {
                                            if (inlineData.TryGetProperty("data", out var imageData))
                                            {
                                                var base64Image = imageData.GetString();
                                                if (!string.IsNullOrEmpty(base64Image))
                                                {
                                                    _logger.LogInformation("Successfully generated image using alternative model: {Model}", model);
                                                    return (base64Image, "Here's the image you requested:", null);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Alternative model {Model} failed: {StatusCode}, {Response}", 
                                model, response.StatusCode, responseContent);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error trying alternative model {Model}", model);
                        continue;
                    }
                }

                // If image generation is not available, return null to use the main error message
                return (null, null, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in TryImagenApiAsync: {Message}", ex.Message);
                return (null, null, null); // Return null to use the main error message
            }
        }

        private async Task<string> GenerateImageDescriptionAsync(string description)
        {
            try
            {
                // Use Gemini to generate a very concise, vivid description
                var prompt = $@"In 2-3 short sentences (max 100 words), describe what an image of '{description}' would look like. Be brief. Mention only the main colors and key visual elements. Keep it concise.";

                var descriptionText = await CallGoogleAIAsync(prompt);
                
                // Trim and clean up the response if it's too long (max 200 characters)
                if (descriptionText.Length > 200)
                {
                    // Split by sentences and take first 2-3 sentences
                    var sentences = descriptionText.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => s.Trim())
                        .Take(3)
                        .ToList();
                    
                    if (sentences.Any())
                    {
                        var trimmed = string.Join(". ", sentences) + ".";
                        // Ensure it's not too long even after trimming
                        if (trimmed.Length > 200)
                        {
                            trimmed = trimmed.Substring(0, 197) + "...";
                        }
                        return trimmed;
                    }
                }
                
                // If still too long, force trim to 200 characters
                if (descriptionText.Length > 200)
                {
                    descriptionText = descriptionText.Substring(0, 197) + "...";
                }
                
                return descriptionText;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating image description: {Message}", ex.Message);
                return $"A vibrant image of {description} with clear colors and professional quality.";
            }
        }
    }
}

