using System.Text.Encodings.Web;

namespace ShopEgypt.Infrastructure.Email;

/// <summary>
/// Simple branded HTML for Identity transactional emails (inline styles for common clients).
/// </summary>
public static class IdentityEmailTemplates
{
    private const string Brand = "ShopEgypt";

    private static string E(string? text) => string.IsNullOrEmpty(text) ? string.Empty : HtmlEncoder.Default.Encode(text);

    public static string ConfirmAccount(string? greetingName, string confirmationUrl)
    {
        var name = string.IsNullOrWhiteSpace(greetingName) ? "there" : greetingName.Trim();
        var href = HtmlEncoder.Default.Encode(confirmationUrl);

        var inner = $@"
<p style=""margin:0 0 16px;font-size:16px;line-height:1.55;color:#1F2126;"">Hi {E(name)},</p>
<p style=""margin:0 0 22px;font-size:15px;line-height:1.65;color:#5f6368;"">Thanks for joining {Brand}. Confirm your email so we know this address is yours and you can sign in.</p>
<table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""margin:0 0 26px;"">
  <tr>
    <td style=""border-radius:999px;background:#495304;"">
      <a href=""{href}"" target=""_blank"" rel=""noopener"" style=""display:inline-block;padding:14px 32px;font-size:15px;font-weight:700;color:#ffffff;text-decoration:none;font-family:Segoe UI,Inter,Roboto,Helvetica,Arial,sans-serif;"">Confirm email</a>
    </td>
  </tr>
</table>
<p style=""margin:0;font-size:13px;line-height:1.6;color:#888888;"">If the button does not work, copy this link into your browser:</p>
<p style=""margin:8px 0 0;font-size:12px;line-height:1.5;word-break:break-all;color:#495304;"">{href}</p>";

        return WrapDocument("Confirm your email", $"{Brand} — confirm your email", inner);
    }

    public static string ResetPassword(string? greetingName, string resetUrl)
    {
        var name = string.IsNullOrWhiteSpace(greetingName) ? "there" : greetingName.Trim();
        var href = HtmlEncoder.Default.Encode(resetUrl);

        var inner = $@"
<p style=""margin:0 0 16px;font-size:16px;line-height:1.55;color:#1F2126;"">Hi {E(name)},</p>
<p style=""margin:0 0 22px;font-size:15px;line-height:1.65;color:#5f6368;"">We received a request to reset your {Brand} password. Use the button below to choose a new one. This link will stop working after a while for your security.</p>
<table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""margin:0 0 26px;"">
  <tr>
    <td style=""border-radius:999px;background:#1F2126;"">
      <a href=""{href}"" target=""_blank"" rel=""noopener"" style=""display:inline-block;padding:14px 32px;font-size:15px;font-weight:700;color:#ffffff;text-decoration:none;font-family:Segoe UI,Inter,Roboto,Helvetica,Arial,sans-serif;"">Reset password</a>
    </td>
  </tr>
</table>
<p style=""margin:0;font-size:13px;line-height:1.6;color:#888888;"">If you did not ask for a reset, you can ignore this email. If the button does not work, paste this link into your browser:</p>
<p style=""margin:8px 0 0;font-size:12px;line-height:1.5;word-break:break-all;color:#495304;"">{href}</p>";

        return WrapDocument("Reset your password", $"{Brand} — password reset", inner);
    }

    private static string WrapDocument(string htmlTitle, string preheader, string innerHtml)
    {
        return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
<meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
<meta charset=""utf-8"" />
<meta name=""viewport"" content=""width=device-width,initial-scale=1"" />
<title>{E(htmlTitle)}</title>
</head>
<body style=""margin:0;padding:0;background-color:#f0ebe2;"">
  <div style=""display:none;font-size:1px;color:#f0ebe2;line-height:1px;max-height:0;max-width:0;opacity:0;overflow:hidden;"">{E(preheader)}</div>
  <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" style=""background-color:#f0ebe2;padding:36px 12px;"">
    <tr>
      <td align=""center"">
        <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" style=""max-width:520px;background-color:#fffef9;border-radius:20px;border:1px solid rgba(31,33,38,0.08);"">
          <tr>
            <td style=""padding:36px 32px 28px;font-family:Segoe UI,Inter,Roboto,Helvetica,Arial,sans-serif;"">
              <p style=""margin:0 0 8px;font-size:24px;font-weight:700;color:#1F2126;font-family:Georgia,'Times New Roman',serif;letter-spacing:-0.02em;"">{E(Brand)}</p>
              <p style=""margin:0 0 28px;font-size:13px;color:#8D9730;font-weight:600;letter-spacing:0.04em;text-transform:uppercase;"">{E(htmlTitle)}</p>
              {innerHtml}
              <p style=""margin:28px 0 0;padding-top:22px;border-top:1px solid rgba(31,33,38,0.08);font-size:12px;line-height:1.55;color:#aaaaaa;"">This message was sent by {E(Brand)}. If you did not expect it, you can safely ignore it.</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>";
    }
}
