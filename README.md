# Introduction 
This repo contains the project files for both the External Portal and One LogIn.
For more information about One LogIn refer to [GOV.UK One LogIn](https://ofgemsousd.atlassian.net/wiki/spaces/EPlus/pages/4118478851/GOV.UK+One+Login).

# Getting Started
1. Pull latest code from repo.

2. Ensure *Ofgem.GBI.External.Portal.Web* is set as your Start-Up Project.

3. Ensure you have entered your Tools->Options->Azure Service Authentication credentials.  This is needed when accesing values from KeyVault.

4. Login to your Azure account in Developer PowerShell by typing **az login** in the terminal, and then log in to your Azure account.

5. When the application is run, you will be presented with a log in page using the following url: *http://localhost:3000*

6. Click on the 'Log me in' button or similar.

7. A basic authentication prompt should appear - assuming your are using Government Digital Service's (GDS) test environment.  Enter the following:
  - User name: integration-user
  - Password: winter2021

If you do not see this prompt and see a **401 Authorization required** message then see below.

> **401 Authorization required**
>
>When running the application with Edge, clicking on the ‘Log me in’ button may return a ‘401 Authorization required’ message.  This is because ‘basic authentication’ has been removed from Edge’s AuthSchemes policy settings.  To view AuthScheme values enter the following into your edge browser address bar:  edge://policy.
>
>You will need to raise ticket to allow you to modify this policy.  Once the ticket has been approved you can amend the AuthSchemes key value in Registry Editor:
>
    Computer\HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Edge

>Amend from “ntlm,negotiate” to “ntlm,negotiate,basic”.  Restart both VS and Edge.

8. If you have been successful, you will be redirected to GDS's test site.

    https://signin.integration.account.gov.uk/sign-in-or-create

9. Create or Sign in using your GOV.UK One Login credentials.






