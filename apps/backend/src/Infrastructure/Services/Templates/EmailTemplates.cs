namespace Infrastructure.Services.Templates;

public static class EmailTemplates
{
    public const string ProfileApproved =
        @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Perfil Aprobado</title>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background: #28a745; color: white; text-align: center; padding: 20px; border-radius: 8px 8px 0 0; }
        .content { background: #f8f9fa; padding: 30px; border-radius: 0 0 8px 8px; }
        .button { display: inline-block; background: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; margin: 20px 0; }
        .footer { text-align: center; margin-top: 30px; font-size: 12px; color: #666; }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>¡Perfil Aprobado!</h1>
        </div>
        <div class='content'>
            <p>Hola <strong>@Model.RecipientName</strong>,</p>
            
            <p>¡Excelentes noticias! Tu perfil ha sido aprobado por nuestro equipo de revisión.</p>
            
            @Model.SfiaLevelSection
            
            @Model.NotesSection
            
            <p>Ya puedes acceder a todas las funcionalidades de la plataforma y ser asignado a equipos.</p>
            
            <a href='@Model.DashboardUrl' class='button'>Ir al Dashboard</a>
            
            <p>¡Bienvenido oficialmente al equipo!</p>
        </div>
        <div class='footer'>
            <p>© 2025 @Model.CompanyName. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";

    public const string ProfileRejected =
        @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Perfil Requiere Actualización</title>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background: #dc3545; color: white; text-align: center; padding: 20px; border-radius: 8px 8px 0 0; }
        .content { background: #f8f9fa; padding: 30px; border-radius: 0 0 8px 8px; }
        .button { display: inline-block; background: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; margin: 20px 0; }
        .footer { text-align: center; margin-top: 30px; font-size: 12px; color: #666; }
        .warning { background: #fff3cd; padding: 15px; border-left: 4px solid #ffc107; margin: 20px 0; }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Actualización Requerida</h1>
        </div>
        <div class='content'>
            <p>Hola <strong>@Model.RecipientName</strong>,</p>
            
            <p>Hemos revisado tu perfil y necesitamos que realices algunas actualizaciones antes de poder aprobarlo.</p>
            
            <div class='warning'>
                <p><strong>Motivo:</strong></p>
                <p>@Model.RejectionReason</p>
            </div>
            
            <p>Por favor, accede a tu perfil y realiza las correcciones necesarias. Una vez actualizadas, podrás solicitar una nueva revisión.</p>
            
            <a href='@Model.DashboardUrl' class='button'>Actualizar Perfil</a>
            
            <p>Si tienes alguna pregunta, no dudes en contactarnos.</p>
        </div>
        <div class='footer'>
            <p>© 2025 @Model.CompanyName. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";

    public const string TeamDeleted =
        @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Equipo Disuelto</title>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background: #fd7e14; color: white; text-align: center; padding: 20px; border-radius: 8px 8px 0 0; }
        .content { background: #f8f9fa; padding: 30px; border-radius: 0 0 8px 8px; }
        .button { display: inline-block; background: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; margin: 20px 0; }
        .footer { text-align: center; margin-top: 30px; font-size: 12px; color: #666; }
        .info { background: #d1ecf1; padding: 15px; border-left: 4px solid #17a2b8; margin: 20px 0; }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Equipo Disuelto</h1>
        </div>
        <div class='content'>
            <p>Hola <strong>@Model.RecipientName</strong>,</p>
            
            <p>Te informamos que el equipo <strong>@Model.TeamName</strong> ha sido disuelto.</p>
            
            <div class='info'>
                <p><strong>Manager del equipo:</strong> @Model.ManagerEmail</p>
            </div>
            
            <p>Esto significa que ya no formas parte de este equipo y estarás disponible para ser asignado a nuevos proyectos.</p>
            
            <a href='@Model.DashboardUrl' class='button'>Ver Dashboard</a>
            
            <p>Si tienes alguna pregunta sobre esta decisión, puedes contactar directamente con tu manager o con el equipo de recursos humanos.</p>
        </div>
        <div class='footer'>
            <p>© 2025 @Model.CompanyName. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";

    public const string TeamMemberAdded =
        @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Agregado a Equipo</title>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background: #28a745; color: white; text-align: center; padding: 20px; border-radius: 8px 8px 0 0; }
        .content { background: #f8f9fa; padding: 30px; border-radius: 0 0 8px 8px; }
        .button { display: inline-block; background: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; margin: 20px 0; }
        .footer { text-align: center; margin-top: 30px; font-size: 12px; color: #666; }
        .success { background: #d4edda; padding: 15px; border-left: 4px solid #28a745; margin: 20px 0; }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>¡Bienvenido al Equipo!</h1>
        </div>
        <div class='content'>
            <p>Hola <strong>@Model.RecipientName</strong>,</p>
            
            <p>¡Excelentes noticias! Has sido agregado al equipo <strong>@Model.TeamName</strong>.</p>
            
            <div class='success'>
                <p><strong>Manager del equipo:</strong> @Model.ManagerEmail</p>
            </div>
            
            <p>Podrás comenzar a colaborar con tu nuevo equipo de inmediato. Te recomendamos revisar los objetivos actuales y ponerte en contacto con tu manager para conocer más detalles sobre tus responsabilidades.</p>
            
            <a href='@Model.DashboardUrl' class='button'>Ver Mi Equipo</a>
            
            <p>¡Esperamos que tengas una experiencia exitosa trabajando con tu nuevo equipo!</p>
        </div>
        <div class='footer'>
            <p>© 2025 @Model.CompanyName. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";

    public const string TeamMemberRemoved =
        @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Removido del Equipo</title>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background: #dc3545; color: white; text-align: center; padding: 20px; border-radius: 8px 8px 0 0; }
        .content { background: #f8f9fa; padding: 30px; border-radius: 0 0 8px 8px; }
        .button { display: inline-block; background: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; margin: 20px 0; }
        .footer { text-align: center; margin-top: 30px; font-size: 12px; color: #666; }
        .warning { background: #f8d7da; padding: 15px; border-left: 4px solid #dc3545; margin: 20px 0; }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Cambio en Asignación de Equipo</h1>
        </div>
        <div class='content'>
            <p>Hola <strong>@Model.RecipientName</strong>,</p>
            
            <p>Te informamos que has sido removido del equipo <strong>@Model.TeamName</strong>.</p>
            
            <div class='warning'>
                <p><strong>Manager del equipo:</strong> @Model.ManagerEmail</p>
            </div>
            
            <p>Esta decisión puede deberse a una reestructuración del equipo, finalización del proyecto, o una reasignación estratégica. Estarás disponible para ser asignado a nuevos proyectos.</p>
            
            <a href='@Model.DashboardUrl' class='button'>Ver Dashboard</a>
            
            <p>Si tienes alguna pregunta sobre esta decisión, puedes contactar directamente con tu manager o con el equipo de recursos humanos.</p>
        </div>
        <div class='footer'>
            <p>© 2025 @Model.CompanyName. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";

    public const string TeamMemberMoved =
        @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Movido a Nuevo Equipo</title>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background: #17a2b8; color: white; text-align: center; padding: 20px; border-radius: 8px 8px 0 0; }
        .content { background: #f8f9fa; padding: 30px; border-radius: 0 0 8px 8px; }
        .button { display: inline-block; background: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; margin: 20px 0; }
        .footer { text-align: center; margin-top: 30px; font-size: 12px; color: #666; }
        .info { background: #d1ecf1; padding: 15px; border-left: 4px solid #17a2b8; margin: 20px 0; }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Reasignación de Equipo</h1>
        </div>
        <div class='content'>
            <p>Hola <strong>@Model.RecipientName</strong>,</p>
            
            <p>Te informamos que has sido movido de <strong>@Model.FromTeamName</strong> a <strong>@Model.ToTeamName</strong>.</p>
            
            <div class='info'>
                <p><strong>Equipo anterior:</strong> @Model.FromTeamName</p>
                <p><strong>Nuevo equipo:</strong> @Model.ToTeamName</p>
                <p><strong>Manager:</strong> @Model.ManagerEmail</p>
            </div>
            
            <p>Esta reasignación se debe a las necesidades actuales del proyecto y tus habilidades. Te recomendamos ponerte en contacto con tu nuevo manager para conocer más detalles sobre tus nuevas responsabilidades.</p>
            
            <a href='@Model.DashboardUrl' class='button'>Ver Nuevo Equipo</a>
            
            <p>¡Esperamos que tengas una experiencia exitosa en tu nuevo equipo!</p>
        </div>
        <div class='footer'>
            <p>© 2025 @Model.CompanyName. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";

    public const string Invitation =
        @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Invitación a Jala Match</title>
    <style>y
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background: #6f42c1; color: white; text-align: center; padding: 20px; border-radius: 8px 8px 0 0; }
        .content { background: #f8f9fa; padding: 30px; border-radius: 0 0 8px 8px; }
        .button { display: inline-block; background: #28a745; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; font-weight: bold; }
        .footer { text-align: center; margin-top: 30px; font-size: 12px; color: #666; }
        .highlight { background: #e7f3ff; padding: 15px; border-left: 4px solid #007bff; margin: 20px 0; border-radius: 4px; }
        .steps { background: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0; }
        .steps ol { margin: 0; padding-left: 20px; }
        .steps li { margin: 10px 0; }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>¡Bienvenido a Jala Match!</h1>
            <p>Has sido invitado a formar parte de nuestro equipo</p>
        </div>
        <div class='content'>
            <p>Hola <strong>@Model.RecipientName</strong>,</p>
            
            <p>¡Tenemos excelentes noticias! Has sido invitado por <strong>@Model.AdminEmail</strong> para unirte a <strong>@Model.CompanyName</strong> a través de nuestra plataforma Jala Match.</p>
            
            <div class='highlight'>
                <p><strong>¿Qué es Jala Match?</strong></p>
                <p>Es nuestra plataforma innovadora de gestión de equipos que utiliza inteligencia artificial para formar equipos altamente compatibles y productivos basados en habilidades técnicas, personalidad y experiencia.</p>
            </div>
            
            <p>Para completar tu registro y comenzar a formar parte de nuestros equipos, sigue estos pasos:</p>
            
            <div class='steps'>
                <ol>
                    <li>Haz clic en el botón verde a continuación</li>
                    <li>Registrate en la aplicación con el mismo correo que nos proporcionaste </li>
                    <li>¡Comienza a gestionar y generar equipos!</li>
                </ol>
            </div>
            
            <div style='text-align: center;'>
                <a href='@Model.InvitationLink' class='button'>Aceptar Invitación</a>
            </div>
            
            <p><strong>Nota importante:</strong> Este enlace de invitación expirará en 7 días, así que asegúrate de completar tu registro pronto.</p>
            
            <p>Si tienes alguna pregunta sobre el proceso de registro o sobre la plataforma, no dudes en contactar a tu administrador en <a href='mailto:@Model.AdminEmail'>@Model.AdminEmail</a>.</p>
            
            <p>¡Esperamos trabajar contigo y verte formar parte de equipos excepcionales!</p>
        </div>
        <div class='footer'>
            <p>© 2025 @Model.CompanyName. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";

    public const string PasswordReset =
        @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Restablecer Contraseña - Jala Match</title>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background: #dc3545; color: white; text-align: center; padding: 20px; border-radius: 8px 8px 0 0; }
        .content { background: #f8f9fa; padding: 30px; border-radius: 0 0 8px 8px; }
        .button { display: inline-block; background: #28a745; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; font-weight: bold; }
        .footer { text-align: center; margin-top: 30px; font-size: 12px; color: #666; }
        .warning { background: #fff3cd; padding: 15px; border-left: 4px solid #ffc107; margin: 20px 0; border-radius: 4px; }
        .security { background: #f8d7da; padding: 15px; border-left: 4px solid #dc3545; margin: 20px 0; border-radius: 4px; }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🔐 Restablecer Contraseña</h1>
            <p>Solicitud de cambio de contraseña</p>
        </div>
        <div class='content'>
            <p>Hola <strong>@Model.RecipientName</strong>,</p>
            
            <p>Hemos recibido una solicitud para restablecer la contraseña de tu cuenta en <strong>@Model.CompanyName</strong>.</p>
            
            <div class='warning'>
                <p><strong>⚠️ Importante:</strong> Si no solicitaste este cambio de contraseña, puedes ignorar este correo de forma segura. Tu contraseña actual seguirá siendo válida.</p>
            </div>
            
            <p>Para restablecer tu contraseña, haz clic en el siguiente botón:</p>
            
            <div style='text-align: center;'>
                <a href='@Model.ResetLink' class='button'>Restablecer Contraseña</a>
            </div>
            
            <p>O puedes copiar y pegar el siguiente enlace en tu navegador:</p>
            <p style='word-break: break-all; background: #e9ecef; padding: 10px; border-radius: 4px; font-family: monospace; font-size: 12px;'>@Model.ResetLink</p>
            
            <div class='security'>
                <p><strong>🛡️ Información de seguridad:</strong></p>
                <ul style='margin: 0; padding-left: 20px;'>
                    <li>Este enlace expirará en 1 hora por tu seguridad</li>
                    <li>Solo puedes usar este enlace una vez</li>
                    <li>Si no fuiste tú quien solicitó esto, cambia tu contraseña inmediatamente</li>
                </ul>
            </div>
            
            <p>Si tienes problemas con el enlace o necesitas ayuda, contacta a nuestro equipo de soporte.</p>
            
            <p>Saludos,<br>El equipo de <strong>@Model.CompanyName</strong></p>
        </div>
        <div class='footer'>
            <p>© 2025 @Model.CompanyName. Todos los derechos reservados.</p>
            <p>Este es un correo automático, por favor no respondas a esta dirección.</p>
        </div>
    </div>
</body>
</html>";
}
