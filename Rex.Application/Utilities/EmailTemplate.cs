namespace Rex.Application.Utilities;

public static class EmailTemplate
{
    public static string ConfirmAccountTemplate(string firstName, string lastName, string code)
    {
	    return $@"<!DOCTYPE html>
		<html lang=""en"">

		<head>
			<meta charset=""UTF-8"">
			<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
			<title>Confirm Your Rex Account</title>
			<style>
				body {{
					margin: 0;
					padding: 0;
					font-family: Arial, Helvetica, sans-serif;
					background-color: #f3f4f6;
					color: #1f2937;
					line-height: 1.6;
				}}

				.container {{
					max-width: 600px;
					margin: 40px auto;
					background-color: #ffffff;
					border-radius: 16px;
					overflow: hidden;
					box-shadow: 0 6px 16px rgba(0, 0, 0, 0.08);
				}}

				.hero {{
					background: linear-gradient(135deg, #EF4444 0%, #F97316 100%);
					padding: 40px 20px;
					color: white;
					text-align: center;
					border-bottom-left-radius: 16px;
					border-bottom-right-radius: 16px;
				}}

				.hero-title {{
					font-size: 30px;
					font-weight: bold;
					margin: 0 0 15px 0;
				}}

				.hero-subtitle {{
					font-size: 18px;
					margin: 0;
					font-weight: 500;
					opacity: 0.95;
				}}

				.content {{
					padding: 30px;
				}}

				.text {{
					font-size: 17px;
					margin: 0 0 20px 0;
					font-weight: 500;
					color: #111827;
				}}

				.code-container {{
					background-color: #f9fafb;
					border-radius: 12px;
					padding: 25px;
					text-align: center;
					margin: 25px 0;
					border: 1px solid #e5e7eb;
					box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
				}}

				.code {{
					font-size: 34px;
					font-weight: bold;
					letter-spacing: 6px;
					color: #EF4444;
					margin: 15px 0;
					padding: 12px 18px;
					background-color: #ffffff;
					border: 2px dashed #EF4444;
					border-radius: 12px;
					display: inline-block;
				}}

				.code-expiry {{
					font-size: 14px;
					color: #6b7280;
					margin: 10px 0 0 0;
					font-weight: 500;
				}}

				.features {{
					background-color: #f9fafb;
					border-radius: 12px;
					padding: 20px;
					margin-top: 30px;
				}}

				.feature {{
					margin-bottom: 15px;
					font-weight: 500;
					display: flex;
					align-items: center;
				}}

				.feature-icon {{
					width: 24px;
					height: 24px;
					background-color: #EF4444;
					border-radius: 50%;
					display: inline-block;
					text-align: center;
					color: white;
					line-height: 24px;
					font-weight: bold;
					margin-right: 10px;
				}}

				@media only screen and (max-width: 480px) {{
					.container {{
						width: 100% !important;
						border-radius: 0;
					}}

					.hero-title {{
						font-size: 22px;
					}}

					.code {{
						font-size: 24px;
						letter-spacing: 3px;
						padding: 8px;
					}}
				}}
			</style>
		</head>

		<body>
			<center>
				<table border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"">
					<tr>
						<td align=""center"" valign=""top"">
							<table class=""container"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
								<tr>
									<td class=""hero"">
										<h1 class=""hero-title"">Confirm Your Rex Account</h1>
										<p class=""hero-subtitle"">You're one step away from pushing your limits</p>
									</td>
								</tr>
								<tr>
									<td class=""content"">
										<p class=""text"">Hello {firstName} {lastName},</p>
										<p class=""text"">Thank you for registering with Rex. To complete your registration, please use the
											following verification code:</p>
										<div class=""code-container"">
											<p style=""margin: 0 0 10px 0; color: #6b7280; font-weight: 500;"">Your confirmation code is:</p>
											<div class=""code"">{code}</div>
											<p class=""code-expiry"">This code will expire in 15 minutes for security reasons.</p>
										</div>
										<p class=""text"">If you didn't create a Rex account, please ignore this message.</p>
										<div class=""features"">
											<div class=""feature"">
												<span class=""feature-icon"">✓</span>
												<span>Personalized challenges</span>
											</div>
											<div class=""feature"">
												<span class=""feature-icon"">✓</span>
												<span>Supportive community</span>
											</div>
											<div class=""feature"">
												<span class=""feature-icon"">✓</span>
												<span>Progress tracking</span>
											</div>
										</div>
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</center>
		</body>

		</html>";
    }
    
    public static string ConfirmEmailChangeTemplate(string firstName, string email,
	    string newEmail, string code)
    {
	    return @"<!DOCTYPE html>
	<html lang=""en"">

	<head>
	    <meta charset=""UTF-8"">
	    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
	    <title>Confirm Your Email Change - Rex</title>
	    <style>
	        body {
	            margin: 0;
	            padding: 0;
	            font-family: Arial, Helvetica, sans-serif;
	            background-color: #f3f4f6;
	            color: #1f2937;
	            line-height: 1.6;
	        }

	        .container {
	            max-width: 600px;
	            margin: 40px auto;
	            background-color: #ffffff;
	            border-radius: 16px;
	            overflow: hidden;
	            box-shadow: 0 6px 16px rgba(0, 0, 0, 0.08);
	        }

	        .hero {
	            background: linear-gradient(135deg, #EF4444 0%, #F97316 100%);
	            padding: 40px 20px;
	            color: white;
	            text-align: center;
	            border-bottom-left-radius: 16px;
	            border-bottom-right-radius: 16px;
	        }

	        .hero-title {
	            font-size: 30px;
	            font-weight: bold;
	            margin: 0 0 15px 0;
	        }

	        .hero-subtitle {
	            font-size: 18px;
	            margin: 0;
	            font-weight: 500;
	            opacity: 0.95;
	        }

	        .content {
	            padding: 30px;
	        }

	        .text {
	            font-size: 17px;
	            margin: 0 0 20px 0;
	            font-weight: 500;
	            color: #111827;
	        }

	        .highlight {
	            color: #EF4444;
	            font-weight: bold;
	        }

	        .code-container {
	            background-color: #f9fafb;
	            border-radius: 12px;
	            padding: 25px;
	            text-align: center;
	            margin: 25px 0;
	            border: 1px solid #e5e7eb;
	            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
	        }

	        .code {
	            font-size: 34px;
	            font-weight: bold;
	            letter-spacing: 6px;
	            color: #EF4444;
	            margin: 15px 0;
	            padding: 12px 18px;
	            background-color: #ffffff;
	            border: 2px dashed #EF4444;
	            border-radius: 12px;
	            display: inline-block;
	        }

	        .code-expiry {
	            font-size: 14px;
	            color: #6b7280;
	            margin: 10px 0 0 0;
	            font-weight: 500;
	        }

	        .warning {
	            background-color: #FEF3C7;
	            border-left: 4px solid #F59E0B;
	            padding: 15px;
	            margin: 20px 0;
	            border-radius: 4px;
	        }

	        .warning-text {
	            margin: 0;
	            color: #92400E;
	            font-weight: 500;
	        }

	        .features {
	            background-color: #f9fafb;
	            border-radius: 12px;
	            padding: 20px;
	            margin-top: 30px;
	        }

	        .feature {
	            margin-bottom: 15px;
	            font-weight: 500;
	            display: flex;
	            align-items: center;
	        }

	        .feature-icon {
	            width: 24px;
	            height: 24px;
	            background-color: #EF4444;
	            border-radius: 50%;
	            display: inline-block;
	            text-align: center;
	            color: white;
	            line-height: 24px;
	            font-weight: bold;
	            margin-right: 10px;
	        }

	        @media only screen and (max-width: 480px) {
	            .container {
	                width: 100% !important;
	                border-radius: 0;
	            }

	            .hero-title {
	                font-size: 22px;
	            }

	            .code {
	                font-size: 24px;
	                letter-spacing: 3px;
	                padding: 8px;
	            }
	        }
	    </style>
	</head>

	<body>
	    <center>
	        <table border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"">
	            <tr>
	                <td align=""center"" valign=""top"">
	                    <table class=""container"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
	                        <tr>
	                            <td class=""hero"">
	                                <h1 class=""hero-title"">Confirm Your Email Change</h1>
	                                <p class=""hero-subtitle"">Security verification for your Rex account</p>
	                            </td>
	                        </tr>
	                        <tr>
	                            <td class=""content"">
	                                <p class=""text"">Hello [Name],</p>
	                                <p class=""text"">You have requested to change the email address associated with your Rex account from <span class=""highlight"">[Old Email]</span> to <span class=""highlight"">[New Email]</span>.</p>
	                                
	                                <div class=""warning"">
	                                    <p class=""warning-text"">⚠️ For security reasons, we need to verify that you authorized this change. If you didn't request this change, please contact our support team immediately.</p>
	                                </div>
	                                
	                                <p class=""text"">To complete the email change process, please use the following verification code:</p>
	                                
	                                <div class=""code-container"">
	                                    <p style=""margin: 0 0 10px 0; color: #6b7280; font-weight: 500;"">Your confirmation code is:</p>
	                                    <div class=""code"">[CODE]</div>
	                                    <p class=""code-expiry"">This code will expire in 15 minutes for security reasons.</p>
	                                </div>
	                                
	                                <p class=""text"">Enter this code in the application to complete the email change process.</p>
	                                
	                                <div class=""features"">
	                                    <div class=""feature"">
	                                        <span class=""feature-icon"">✓</span>
	                                        <span>Personalized challenges</span>
	                                    </div>
	                                    <div class=""feature"">
	                                        <span class=""feature-icon"">✓</span>
	                                        <span>Supportive community</span>
	                                    </div>
	                                    <div class=""feature"">
	                                        <span class=""feature-icon"">✓</span>
	                                        <span>Progress tracking</span>
	                                    </div>
	                                </div>
	                                
	                                <p class=""text"">Best regards,<br>The Rex Team</p>
	                            </td>
	                        </tr>
	                    </table>
	                </td>
	            </tr>
	        </table>
	    </center>
	</body>

	</html>";
    }
}