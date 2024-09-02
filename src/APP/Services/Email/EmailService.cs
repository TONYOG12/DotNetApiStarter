/*using System.Net.Mail;
using APP.Utils;
using DOMAIN.Entities;
using DOMAIN.Entities.Incidents;
using DOMAIN.Entities.Users;

//using MimeKit;
//using MailKit.Net.Smtp;

namespace APP.Services.Email;
public class EmailService(IKeyVaultService keyVaultService) : IEmailService
{
    private async Task SendMail(User user, string encode)
    {
        var onTestMode = Environment.GetEnvironmentVariable("RUN_TEST_SEEDS");
        if (onTestMode == "ON")
        {
            return;
        }
        //// Compose a message
        var domainPostfix = Environment.GetEnvironmentVariable("subDomainPostfix");
        MimeMessage mail = new();
        mail.From.Add(new MailboxAddress($"{user.OrganizationName}", $"no-reply@{user.OrganizationName}{domainPostfix}.com"));
        mail.To.Add(new MailboxAddress($"{user.FirstName}", $"{user.Email}"));
        mail.Subject = $"Hello, {user.FirstName}";
        mail.Body = new TextPart("html")
        {
            Text = $"{encode}"
        };

        // Send it!
        var smtpPassword = await keyVaultService.GetSecretAsync("MailGunPassword");
        
        using var client = new SmtpClient();
       
        client.ServerCertificateValidationCallback = (_, _, _, _) => true;

        await client.ConnectAsync("smtp.mailgun.org", 587, false);
        client.AuthenticationMechanisms.Remove("XOAUTH2");
        await client.AuthenticateAsync("postmaster@bemddops.com", $"{smtpPassword}");

        await client.SendAsync(mail);
        await client.DisconnectAsync(true);
    }

    public async Task SendActionEmail(ActionDto action, User employee, string emailType)
    {
        var baseUrl = Environment.GetEnvironmentVariable("clientBaseUrl");
        var url = $"https://{employee.OrganizationName.ToLower()}.{baseUrl}/actions/view/details/{action.Id}";
        var encode = emailType switch
        {
            ActionUtils.Open => $@"
<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>
<html xmlns='http://www.w3.org/1999/xhtml' style='font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'>
<head>
<meta name='viewport' content='width=device-width' />
<meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />
<title></title>
<style type='text/css'>

body {{
-webkit-font-smoothing: antialiased; -webkit-text-size-adjust: none; width: 100% !important; height: 100%; line-height: 1.6em;
}}
body {{
background-color: #f6f6f6;
}}
@media only screen and (max-width: 640px) {{
  body {{
    padding: 0 !important;
  }}
  h1 {{
    font-weight: 800 !important; margin: 20px 0 5px !important;
  }}
  h2 {{
    font-weight: 800 !important; margin: 20px 0 5px !important;
  }}
  h3 {{
    font-weight: 800 !important; margin: 20px 0 5px !important;
  }}
  h4 {{
    font-weight: 800 !important; margin: 20px 0 5px !important;
  }}
  h1 {{
    font-size: 22px !important;
  }}
  h2 {{
    font-size: 18px !important;
  }}
  h3 {{
    font-size: 16px !important;
  }}
  .container {{
    padding: 0 !important; width: 100% !important;
  }}
  .content {{
    padding: 0 !important;
  }}
  .content-wrap {{
    padding: 10px !important;
  }}
  .invoice {{
    width: 100% !important;
  }}
}}
</style>
</head>

<body itemscope itemtype='http://schema.org/EmailMessage' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: none; width: 100% !important; height: 100%; line-height: 1.6em; background-color: #f6f6f6; margin: 0;' bgcolor='#f6f6f6'>

<table class='body-wrap' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; width: 100%; background-color: #f6f6f6; margin: 0;' bgcolor='#f6f6f6'><tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0;' valign='top'></td>
		<td class='container' width='600' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; display: block !important; max-width: 600px !important; clear: both !important; margin: 0 auto;' valign='top'>
			<div class='content' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; max-width: 600px; display: block; margin: 0 auto; padding: 20px;'>
				<table class='main' width='100%' cellpadding='0' cellspacing='0' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; border-radius: 3px; background-color: #fff; margin: 0; border: 1px solid #e9e9e9;' bgcolor='#fff'><tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-wrap' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 20px;' valign='top'>
							<table width='100%' cellpadding='0' cellspacing='0' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'>
                
                <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
										Dear <strong style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'>{employee.Title} {employee.FirstName} {employee.LastName}</strong>,
									</td>
								</tr>
                
                
                <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
								This automated message from the {employee.OrganizationName} Application is to notify you that the action item related to <strong>{action.ModelType} #{action.Model?.Reference} </strong> has been completed.
									</td>
								</tr>
                
                  <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
                    Action Item: <br/>
{action.Description}
									</td>
								</tr>
                
                  <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
The action was completed before the deadline of <strong>{action.DueDate}.</strong> Your review and confirmation of closure are now required.
									</td>
								</tr>
                
                <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
                  Please click on the following link to review the completed action and mark it as closed within the application: : <a href={url}>[Link to action in-app].</a>
									</td>
								</tr>
                
                  <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
                  If you have any queries regarding this action or the associated incident, please contact the relevant HSE officer or supervisor directly.
									</td>
								</tr>
                
                  <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
                  Thank you for your cooperation and dedication to maintaining a safe and healthy work environment.
									</td>
								</tr>
                
                 <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
                   Best regards, <br/> {employee.OrganizationName} 
									</td>
								</tr>
                
                </table></td>
					</tr></table><div class='footer' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; width: 100%; clear: both; color: #999; margin: 0; padding: 20px;'>
					<table width='100%' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'>
						</tr></table></div></div>
		</td>
		<td style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0;' valign='top'></td>
	</tr></table></body>
</html>",
            ActionUtils.Rejected => $@"
<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>
<html xmlns='http://www.w3.org/1999/xhtml' style='font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'>
<head>
<meta name='viewport' content='width=device-width' />
<meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />
<title></title>
<style type='text/css'>

body {{
-webkit-font-smoothing: antialiased; -webkit-text-size-adjust: none; width: 100% !important; height: 100%; line-height: 1.6em;
}}
body {{
background-color: #f6f6f6;
}}
@media only screen and (max-width: 640px) {{
  body {{
    padding: 0 !important;
  }}
  h1 {{
    font-weight: 800 !important; margin: 20px 0 5px !important;
  }}
  h2 {{
    font-weight: 800 !important; margin: 20px 0 5px !important;
  }}
  h3 {{
    font-weight: 800 !important; margin: 20px 0 5px !important;
  }}
  h4 {{
    font-weight: 800 !important; margin: 20px 0 5px !important;
  }}
  h1 {{
    font-size: 22px !important;
  }}
  h2 {{
    font-size: 18px !important;
  }}
  h3 {{
    font-size: 16px !important;
  }}
  .container {{
    padding: 0 !important; width: 100% !important;
  }}
  .content {{
    padding: 0 !important;
  }}
  .content-wrap {{
    padding: 10px !important;
  }}
  .invoice {{
    width: 100% !important;
  }}
}}
</style>
</head>

<body itemscope itemtype='http://schema.org/EmailMessage' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: none; width: 100% !important; height: 100%; line-height: 1.6em; background-color: #f6f6f6; margin: 0;' bgcolor='#f6f6f6'>

<table class='body-wrap' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; width: 100%; background-color: #f6f6f6; margin: 0;' bgcolor='#f6f6f6'><tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0;' valign='top'></td>
		<td class='container' width='600' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; display: block !important; max-width: 600px !important; clear: both !important; margin: 0 auto;' valign='top'>
			<div class='content' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; max-width: 600px; display: block; margin: 0 auto; padding: 20px;'>
				<table class='main' width='100%' cellpadding='0' cellspacing='0' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; border-radius: 3px; background-color: #fff; margin: 0; border: 1px solid #e9e9e9;' bgcolor='#fff'><tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-wrap' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 20px;' valign='top'>
							<table width='100%' cellpadding='0' cellspacing='0' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'>
                
                <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
										Dear <strong style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'>[HSE Manager's Name]</strong>,
									</td>
								</tr>
                
                
                <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
								This automated message from the {employee.OrganizationName} Application is to notify you that the action item related to <strong>{action.ModelType}#{action.Model?.Reference} </strong> has been completed.
									</td>
								</tr>
                
                  <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
                    Action Item: <br/>{action.Description}
									</td>
								</tr>
                
                  <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
The action was completed before the deadline of <strong>{action.DueDate}.</strong> Your review and confirmation of closure are now required.
									</td>
								</tr>
                
                <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
                  Please click on the following link to review the completed action and mark it as closed within the application: : <a href={url}>[Link to action in-app].</a>
									</td>
								</tr>
                
                  <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
                  If you have any queries regarding this action or the associated incident, please contact the relevant HSE officer or supervisor directly.
									</td>
								</tr>
                
                  <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
                  Thank you for your cooperation and dedication to maintaining a safe and healthy work environment.
									</td>
								</tr>
                
                 <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
                   Best regards, <br/> {employee.OrganizationName} 
									</td>
								</tr>
                
                </table></td>
					</tr></table><div class='footer' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; width: 100%; clear: both; color: #999; margin: 0; padding: 20px;'>
					<table width='100%' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'>
						</tr></table></div></div>
		</td>
		<td style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0;' valign='top'></td>
	</tr></table></body>
</html>
",
            ActionUtils.Resolved => $@"
<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>
<html xmlns='http://www.w3.org/1999/xhtml' style='font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'>
<head>
<meta name='viewport' content='width=device-width' />
<meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />
<title></title>
<style type='text/css'>

body {{
-webkit-font-smoothing: antialiased; -webkit-text-size-adjust: none; width: 100% !important; height: 100%; line-height: 1.6em;
}}
body {{
background-color: #f6f6f6;
}}
@media only screen and (max-width: 640px) {{
  body {{
    padding: 0 !important;
  }}
  h1 {{
    font-weight: 800 !important; margin: 20px 0 5px !important;
  }}
  h2 {{
    font-weight: 800 !important; margin: 20px 0 5px !important;
  }}
  h3 {{
    font-weight: 800 !important; margin: 20px 0 5px !important;
  }}
  h4 {{
    font-weight: 800 !important; margin: 20px 0 5px !important;
  }}
  h1 {{
    font-size: 22px !important;
  }}
  h2 {{
    font-size: 18px !important;
  }}
  h3 {{
    font-size: 16px !important;
  }}
  .container {{
    padding: 0 !important; width: 100% !important;
  }}
  .content {{
    padding: 0 !important;
  }}
  .content-wrap {{
    padding: 10px !important;
  }}
  .invoice {{
    width: 100% !important;
  }}
}}
</style>
</head>

<body itemscope itemtype='http://schema.org/EmailMessage' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: none; width: 100% !important; height: 100%; line-height: 1.6em; background-color: #f6f6f6; margin: 0;' bgcolor='#f6f6f6'>

<table class='body-wrap' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; width: 100%; background-color: #f6f6f6; margin: 0;' bgcolor='#f6f6f6'><tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0;' valign='top'></td>
		<td class='container' width='600' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; display: block !important; max-width: 600px !important; clear: both !important; margin: 0 auto;' valign='top'>
			<div class='content' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; max-width: 600px; display: block; margin: 0 auto; padding: 20px;'>
				<table class='main' width='100%' cellpadding='0' cellspacing='0' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; border-radius: 3px; background-color: #fff; margin: 0; border: 1px solid #e9e9e9;' bgcolor='#fff'><tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-wrap' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 20px;' valign='top'>
							<table width='100%' cellpadding='0' cellspacing='0' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'>
                
                <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
										Dear <strong style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'>{employee.Title} {employee.FirstName} {employee.LastName}</strong>,
									</td>
								</tr>
                
                
                <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
									We would like to inform you that the proposed resolution for the action item related to <strong>{action.ModelType} #{action.Model?.Reference} </strong> within <strong>{employee.OrganizationName} </strong> has been reviewed and <strong style='text-transform:uppercase'>rejected</strong> by the HSE Manager.
									</td>
								</tr>
                
                  <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
                    Action Item: <br/>{action.Description}
									</td>
								</tr>
                
                  <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
We kindly request your re-evaluation of the situation and submission of a new proposed action for the incident.
									</td>
								</tr>
                
                <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
                   To review feedback and resubmit the action, please click on the following link: : <a href={url}>[Link to action in-app].</a>
									</td>
								</tr>
                
                  <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
                   Please note that this message is automated. If you have any questions regarding this rejection, please reach out to your HSE officer or supervisor directly.
									</td>
								</tr>
                
                  <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
                   Thank you for your prompt attention to this matter. Your cooperation is greatly appreciated.
									</td>
								</tr>
                
                 <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
                   Best regards, <br/> {employee.OrganizationName} 
									</td>
								</tr>
                
                </table></td>
					</tr></table><div class='footer' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; width: 100%; clear: both; color: #999; margin: 0; padding: 20px;'>
					<table width='100%' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'>
						</tr></table></div></div>
		</td>
		<td style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0;' valign='top'></td>
	</tr></table></body>
</html>
",
            _ => string.Empty
        };

        if (string.IsNullOrEmpty(encode)) return;

        await SendMail(employee, encode);
    }


    public async Task SendNewIncidentEmail(Incident incident, User employee, string url)
    {
        var onTestMode = Environment.GetEnvironmentVariable("RUN_TEST_SEEDS");
        if (onTestMode == "ON")
        {
            return;
        }
        if(employee == null || incident == null) return;
        
            var encode = $@" <!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>
<html xmlns='http://www.w3.org/1999/xhtml' style='font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'>
<head>
<meta name='viewport' content='width=device-width' />
<meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />
<title></title>
<style type='text/css'>

body {{
-webkit-font-smoothing: antialiased; -webkit-text-size-adjust: none; width: 100% !important; height: 100%; line-height: 1.6em;
}}
body {{
background-color: #f6f6f6;
}}
@media only screen and (max-width: 640px) {{
  body {{
    padding: 0 !important;
  }}
  h1 {{
    font-weight: 800 !important; margin: 20px 0 5px !important;
  }}
  h2 {{
    font-weight: 800 !important; margin: 20px 0 5px !important;
  }}
  h3 {{
    font-weight: 800 !important; margin: 20px 0 5px !important;
  }}
  h4 {{
    font-weight: 800 !important; margin: 20px 0 5px !important;
  }}
  h1 {{
    font-size: 22px !important;
  }}
  h2 {{
    font-size: 18px !important;
  }}
  h3 {{
    font-size: 16px !important;
  }}
  .container {{
    padding: 0 !important; width: 100% !important;
  }}
  .content {{
    padding: 0 !important;
  }}
  .content-wrap {{
    padding: 10px !important;
  }}
  .invoice {{
    width: 100% !important;
  }}
}}
</style>
</head>

<body itemscope itemtype='http://schema.org/EmailMessage' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: none; width: 100% !important; height: 100%; line-height: 1.6em; background-color: #f6f6f6; margin: 0;' bgcolor='#f6f6f6'>

<table class='body-wrap' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; width: 100%; background-color: #f6f6f6; margin: 0;' bgcolor='#f6f6f6'><tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0;' valign='top'></td>
		<td class='container' width='600' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; display: block !important; max-width: 600px !important; clear: both !important; margin: 0 auto;' valign='top'>
			<div class='content' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; max-width: 600px; display: block; margin: 0 auto; padding: 20px;'>
				<table class='main' width='100%' cellpadding='0' cellspacing='0' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; border-radius: 3px; background-color: #fff; margin: 0; border: 1px solid #e9e9e9;' bgcolor='#fff'><tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-wrap' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 20px;' valign='top'>
							<table width='100%' cellpadding='0' cellspacing='0' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'>
                
                <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
										Dear <strong style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'>{employee.Title} {employee.FirstName} {employee.LastName}</strong>,
									</td>
								</tr>
                
                
                <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
										This automated message from the <strong>{employee.OrganizationName}</strong> is to inform you of a new action item related to Incident Report <strong>{incident.Number} </strong>.
									</td>
								</tr>
                
                  <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
                    Action Item: This is just a very small brief of the incident. Due: <strong>{incident.CreatedAt}</strong>
									</td>
								</tr>
                
                <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
                    Please review and complete this action here: <a href={url}>Review action link.</a>
									</td>
								</tr>
                
                  <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
                   This is an automated message. For questions, contact your HSE officer or supervisor. For technical issues, contact <a href=''>admin@hsasystems.com.</a>
									</td>
								</tr>
                
                  <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
                   Thank you for your prompt attention.
									</td>
								</tr>
                
                 <tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><td class='content-block' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;' valign='top'>
                   Best, {employee.OrganizationName} 
									</td>
								</tr>
                
                </table></td>
					</tr></table><div class='footer' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; width: 100%; clear: both; color: #999; margin: 0; padding: 20px;'>
					<table width='100%' style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'><tr style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;'>
						</tr></table></div></div>
		</td>
		<td style='font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0;' valign='top'></td>
	</tr></table></body>
</html>
";

            await SendMail(employee, encode);


    }
    
    public async Task SendForgotPasswordEmail(string url, User user)
    {
        if(user != null)
        {
            var encode = $@"
<!--
 This is an example of a simple transactional email. 
 License: MIT
 Credit: Instagram
-->

<body style='margin:0;padding:0' dir='ltr' bgcolor='#ffffff'>
    <table border='0' cellspacing='0' cellpadding='0' align='center' id='m_-7626415423304311386email_table'
        style='border-collapse:collapse'>
        <tbody>
            <tr>
                <td id='m_-7626415423304311386email_content'
                    style='font-family:Helvetica Neue,Helvetica,Lucida Grande,tahoma,verdana,arial,sans-serif;background:#ffffff'>
                    <table border='0' width='100%' cellspacing='0' cellpadding='0' style='border-collapse:collapse'>
                        <tbody>
                            <tr>
                                <td height='20' style='line-height:20px' colspan='3'>&nbsp;</td>
                            </tr>
                            <tr>
                                <td height='1' colspan='3' style='line-height:1px'></td>
                            </tr>
                            <tr>
                                <td>
                                    <table border='0' width='100%' cellspacing='0' cellpadding='0'
                                        style='border-collapse:collapse;text-align:center;width:100%'>
                                        <tbody>
                                            <tr>
                                                <td width='15px' style='width:15px'></td>
                                                <td style='line-height:0px;max-width:600px;padding:0 0 0 0'>
                                                    <table border='0' width='100%' cellspacing='0' cellpadding='0'
                                                        style='border-collapse:collapse'>
                                                        <tbody>
                                                            <tr>
                                                                <td style='width:100%;text-align:left;height:33px'><img
                                                                        height='33'
                                                                        src='https://chimerawebreact.z13.web.core.windows.net/main-logo.png'
                                                                        style='border:0' class='CToWUd' data-bit='iit'>
                                                                </td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </td>
                                                <td width='15px' style='width:15px'></td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table border='0' width='430' cellspacing='0' cellpadding='0'
                                        style='border-collapse:collapse;margin:0 auto 0 auto'>
                                        <tbody>
                                            <tr>
                                                <td>
                                                    <table border='0' width='430px' cellspacing='0' cellpadding='0'
                                                        style='border-collapse:collapse;margin:0 auto 0 auto;width:430px'>
                                                        <tbody>
                                                            <tr>
                                                                <td width='15' style='display:block;width:15px'>
                                                                    &nbsp;&nbsp;&nbsp;</td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <table border='0' width='100%' cellspacing='0'
                                                                        cellpadding='0'
                                                                        style='border-collapse:collapse'>
                                                                        <tbody>
                                                                            <tr>
                                                                                <td>
                                                                                    <table border='0' cellspacing='0'
                                                                                        cellpadding='0'
                                                                                        style='border-collapse:collapse'>
                                                                                        <tbody>
                                                                                            <tr>
                                                                                                <td width='20'
                                                                                                    style='display:block;width:20px'>
                                                                                                    &nbsp;&nbsp;&nbsp;
                                                                                                </td>
                                                                                                <td>
                                                                                                    <table border='0'
                                                                                                        cellspacing='0'
                                                                                                        cellpadding='0'
                                                                                                        style='border-collapse:collapse'>
                                                                                                        <tbody>
                                                                                                            <tr>
                                                                                                                <td>
                                                                                                                    <p
                                                                                                                        style='margin:10px 0 10px 0;color:#565a5c;font-size:18px'>
                                                                                                                        Hi
                                                                                                                        {user.FirstName} 
                                                                                                                        {user.LastName}!
                                                                                                                    </p>
                                                                                                                    <p
                                                                                                                        style='margin:10px 0 10px 0;color:#565a5c;font-size:18px;line-height:30px'>
                                                                                                                        We
                                                                                                                        received
                                                                                                                        a
                                                                                                                        request
                                                                                                                        to
                                                                                                                        reset
                                                                                                                        your
                                                                                                                        password.
                                                                                                                        <br />Use
                                                                                                                        the
                                                                                                                        button
                                                                                                                        below
                                                                                                                        to
                                                                                                                        set
                                                                                                                        your
                                                                                                                        password.
                                                                                                                    </p>
                                                                                                                </td>
                                                                                                            </tr>
                                                                                                            <tr>
                                                                                                                <td>&nbsp;
                                                                                                                </td>
                                                                                                            </tr>
                                                                                                            <tr>
                                                                                                                <td><a href={url}
                                                                                                                        style='color:#1b74e4;text-decoration:none;display:block;width:370px'>
                                                                                                                        <table
                                                                                                                            border='0'
                                                                                                                            width='390'
                                                                                                                            cellspacing='0'
                                                                                                                            cellpadding='0'
                                                                                                                            style='border-collapse:collapse'>
                                                                                                                            <tbody>
                                                                                                                                <tr>
                                                                                                                                    <td
                                                                                                                                        style='border-collapse:collapse;border-radius:3px;text-align:center;display:block;border:solid 1px #c9da2b;padding:10px 16px 14px 16px;margin:0 0 0 auto;min-width:80px;background-color:#c9da2b'>
                                                                                                                                        <a href={url}
                                                                                                                                            style='color:#1b74e4;text-decoration:none;display:block'>
                                                                                                                                            <center>
                                                                                                                                                <font
                                                                                                                                                    size='3'>
                                                                                                                                                    <span
                                                                                                                                                        style='font-family:Helvetica Neue,Helvetica,Roboto,Arial,sans-serif;white-space:nowrap;font-weight:bold;vertical-align:middle;color:#0f202d;font-size:16px;line-height:16px;letter-spacing:4px'>
                                                                                                                                                        Click
                                                                                                                                                        Here
                                                                                                                                                </font>
                                                                                                                                            </center>
                                                                                                                                        </a>
                                                                                                                                    </td>
                                                                                                                                </tr>

                                                                                                                                <tr>
                                                                                                                                    <td>

                                                                                                                                        <p
                                                                                                                                            style='margin:10px 0 10px 0;color:#565a5c;font-size:18px'>
                                                                                                                                            <small>
                                                                                                                                                If
                                                                                                                                                you
                                                                                                                                                did
                                                                                                                                                not
                                                                                                                                                request
                                                                                                                                                to
                                                                                                                                                change
                                                                                                                                                your
                                                                                                                                                password,
                                                                                                                                                ignore
                                                                                                                                                this
                                                                                                                                                email
                                                                                                                                                and
                                                                                                                                                the
                                                                                                                                                link
                                                                                                                                                will
                                                                                                                                                expire
                                                                                                                                                on
                                                                                                                                                its
                                                                                                                                                own.
                                                                                                                                            </small>
                                                                                                                                        </p>
                                                                                                                                    </td>
                                                                                                                                </tr>
                                                                                                                            </tbody>
                                                                                                                        </table>
                                                                                                                    </a>
                                                                                                                </td>
                                                                                                            </tr>
                                                                                                            <tr>
                                                                                                                <td height='20'
                                                                                                                    style='line-height:20px'>
                                                                                                                    &nbsp;
                                                                                                                </td>
                                                                                                            </tr>
                                                                                                        </tbody>
                                                                                                    </table>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </tbody>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>
                                                                        </tbody>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td height='10' style='line-height:10px' colspan='1'>
                                                                    &nbsp;</td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>

                                </td>
                            </tr>
                            <tr>
                                <td height='20' style='line-height:20px' colspan='3'>&nbsp;</td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
        </tbody>
    </table>
</body>";

            await SendMail(user, encode);

        }
    }

    public async Task WelcomeEmail(User user)
    {
        if (user != null)
        {
            var encode = $@"";

            await SendMail(user, encode);
        }
    }
}*/