namespace MiniLojaVirtual.Service.EmailSender.Constants;

public static partial class EmailTemplates
{
	internal static string EmailConfirmationLinkHtmlMessage(string logoUrl, string confirmationLink)
	{
		return @$"
		<!DOCTYPE html>
		<html lang=""pt-BR"">
		<head>
			<meta charset=""UTF-8"" />
			<title>Confirmação de E-mail</title>
			{DefaultStyle}
		</head>
		<body>
			<div class=""email-container"">
				{DefaultHeader(logoUrl)}

				<div class=""email-body"">
					<h1>Olá!</h1>
					<p>
						Obrigado por se cadastrar em nossa plataforma. Para ativar a sua conta, é necessário confirmar o seu endereço de e-mail. 
						Basta clicar no botão abaixo para finalizar o processo de ativação.
					</p>
					<p>
						Se você não reconhece este cadastro, por favor desconsidere este e-mail.
					</p>

					<p style=""text-align: center;"">
						<a href=""{confirmationLink}"" class=""email-button"" target=""_blank"">Confirmar E-mail</a>
					</p>
					<p>
						Caso o botão não funcione, copie e cole o link abaixo em seu navegador:
						<br />
						<strong>{confirmationLink}</strong>
					</p>
				</div>

				{DefaultFooter}
			</div>
		</body>
		</html>
	";
	}

	internal static string EmailPasswordResetLinkHtmlMessage(string logoUrl, string resetLink)
	{
		return @$"
		<!DOCTYPE html>
		<html lang=""pt-BR"">
		<head>
			<meta charset=""UTF-8"" />
			<title>Redefinição de Senha</title>
			{DefaultStyle}
		</head>
			<body>
				<div class=""email-container"">
					{DefaultHeader(logoUrl)}

					<div class=""email-body"">
						<h1>Olá!</h1>
						<p>
							Recebemos uma solicitação para redefinir a sua senha. Se foi você quem solicitou, basta clicar no botão abaixo para criar uma nova senha.
						</p>
						<p>
							Caso não tenha sido você, por favor ignore este e-mail. Sua senha permanecerá inalterada.
						</p>

						<p style=""text-align: center;"">
							<a href=""{resetLink}"" class=""email-button"" target=""_blank"">Redefinir Senha</a>
						</p>
						<p>
							Caso o botão não funcione, copie e cole o link abaixo em seu navegador:
							<br />
							<strong>{resetLink}</strong>
						</p>
					</div>

					{DefaultFooter}
				</div>
			</body>
		";
	}

	internal static string EmailPasswordResetCodeHtmlMessage(string logoUrl, string resetCode)
	{
		return @$"
			<!DOCTYPE html>
			<html lang=""pt-BR"">
			<head>
				<meta charset=""UTF-8"" />
				<title>Redefinição de Senha</title>
				{DefaultStyle}
			</head>
			<body>
				<div class=""email-container"">
					{DefaultHeader(logoUrl)}

					<div class=""email-body"">
						<h1>Olá!</h1>
						<p>
							Recebemos uma solicitação para redefinir a sua senha. Se foi você quem solicitou, utilize o código abaixo para criar uma nova senha.
						</p>
						<p>
							Caso não tenha sido você, por favor ignore este e-mail. Sua senha permanecerá inalterada.
						</p>

						<p style=""text-align: center; font-size: 24px; font-weight: bold; margin: 30px 0;"">
							{resetCode}
						</p>

						<p>
							Este código expira em 5 minutos. Se o código não for utilizado nesse período, será necessário solicitar novamente.
						</p>
					</div>

					{DefaultFooter}
				</div>
			</body>
			</html>
		";
	}

	private static string DefaultHeader(string logoUrl)
	{
		return $"""
		        	<div class=""email-header"">
		        		<img src=""{logoUrl}"" alt=""Lobo Inc."" />
		        		<h2 class=""email-title"">Confirmação de E-mail</h2>
		        	</div>
		        """;
	}

	private static readonly string DefaultFooter =
		$"""
		 <div class="email-footer">
		 	<p>
		 		&copy; {DateTime.UtcNow.Year} - Lobo Inc. Todos os direitos reservados.
		 	</p>
		 </div>
		 """;

	private const string DefaultStyle =
		"""
			< style>
				body, table, td, a {
					margin: 0;
					padding: 0;
					text-decoration: none;
					font-family: Arial, sans-serif;
				}

				img {
					border: none;
				}

				body {
					background - color: #f4f4f4;
					color: #333333;
				}

				.email-container {
					max - width: 600px;
					margin: 0 auto;
					background-color: #ffffff;
					border-radius: 8px;
					overflow: hidden;
					box-shadow: 0 2px 5px rgba(0,0,0,0.1);
				}

				.email-header {
					background - color: #1F2937;
					padding: 20px;
					text-align: center;
				}

				.email-header img {
					max - width: 150px;
					margin-bottom: 10px;
				}

				.email-title {
					font - size: 24px;
					font-weight: bold;
					color: #ffffff;
					margin: 0;
				}

				.email-body {
					padding: 20px;
				}
				
				.email-body h1 {
					font - size: 20px;
					margin-top: 0;
					color: #333333;
				}
				
				.email-body p {
					font - size: 16px;
					line-height: 1.6;
					margin-bottom: 10px;
				}

				.email-button {
					display: inline-block;
					padding: 12px 20px;
					background-color: #3B82F6;
					color: #ffffff;
					border-radius: 4px;
					font-size: 16px;
					font-weight: bold;
					margin: 20px 0;
				}
				
				.email-button:hover {
					background - color: #2563EB;
				}

				.email-footer {
					text - align: center;
					font-size: 14px;
					color: #777777;
					border-top: 1px solid #eaeaea;
					padding: 20px;
				}

				@media screen and (max-width: 600px) {
					.email - body, .email - header, .email-footer {
						padding: 15px;
					}
					
					.email-title {
						font-size: 20px;
					}

					.email-button {
						width: 100%;
						text-align: center;
					}
				}
			</style>
		""";
}