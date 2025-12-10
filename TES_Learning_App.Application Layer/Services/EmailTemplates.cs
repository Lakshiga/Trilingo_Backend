namespace TES_Learning_App.Application_Layer.Services
{
    public static class EmailTemplates
    {
        public static string GenerateOtpEmailTemplate(string otp, string username)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Password Reset OTP</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;'>
    <div style='background-color: #f4f4f4; padding: 20px; border-radius: 5px;'>
        <h2 style='color: #2c3e50;'>Password Reset Request</h2>
        <p>Hello <strong>{username}</strong>,</p>
        <p>You have requested to reset your password for your TES Learning App account.</p>
        <div style='background-color: #fff; padding: 20px; margin: 20px 0; border-radius: 5px; border: 2px solid #3498db; text-align: center;'>
            <p style='margin: 0; font-size: 14px; color: #666;'>Your OTP code is:</p>
            <h1 style='color: #3498db; font-size: 36px; letter-spacing: 5px; margin: 10px 0;'>{otp}</h1>
        </div>
        <p><strong>This code will expire in 15 minutes.</strong></p>
        <p>If you did not request this password reset, please ignore this email or contact support if you have concerns.</p>
        <p style='margin-top: 30px; font-size: 12px; color: #999;'>This is an automated message, please do not reply to this email.</p>
    </div>
</body>
</html>";
        }

        public static string GenerateResetLinkEmailTemplate(string resetLink, string username)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Password Reset Link</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;'>
    <div style='background-color: #f4f4f4; padding: 20px; border-radius: 5px;'>
        <h2 style='color: #2c3e50;'>Password Reset Request</h2>
        <p>Hello <strong>{username}</strong>,</p>
        <p>You have requested to reset your password for your TES Learning App account.</p>
        <p>Click the button below to reset your password:</p>
        <div style='text-align: center; margin: 30px 0;'>
            <a href='{resetLink}' style='background-color: #3498db; color: #fff; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block; font-weight: bold;'>Reset Password</a>
        </div>
        <p>Or copy and paste this link into your browser:</p>
        <p style='word-break: break-all; color: #3498db;'>{resetLink}</p>
        <p><strong>This link will expire in 1 hour.</strong></p>
        <p>If you did not request this password reset, please ignore this email or contact support if you have concerns.</p>
        <p style='margin-top: 30px; font-size: 12px; color: #999;'>This is an automated message, please do not reply to this email.</p>
    </div>
</body>
</html>";
        }
    }
}

