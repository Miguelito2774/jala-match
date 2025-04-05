import { StaticPageLayout } from '@/components/templates/StaticPageLayout';

export default function PrivacyPage() {
  return (
    <StaticPageLayout title="Política de Privacidad">
      <h2>1. Recopilación de Información</h2>
      <p>
        Recopilamos la información que proporcionas directamente, como datos de perfil, habilidades técnicas,
        experiencia laboral e intereses personales.
      </p>

      <h2>2. Uso de la Información</h2>
      <p>
        Utilizamos tu información para proporcionar y mejorar nuestros servicios, incluida la formación de equipos
        compatibles mediante algoritmos de matchmaking.
      </p>

      <h2>3. Compartir Información</h2>
      <p>
        Tu información solo será visible para los empleados, managers y administradores autorizados dentro de la
        plataforma.
      </p>

      <h2>4. Seguridad de Datos</h2>
      <p>
        Implementamos medidas de seguridad para proteger tu información personal contra acceso no autorizado o
        modificación.
      </p>

      <h2>5. Derechos del Usuario</h2>
      <p>
        Tienes derecho a acceder, corregir o eliminar tu información personal. Puedes hacerlo a través de la sección de
        configuración de tu perfil.
      </p>

      <h2>6. Actualizaciones de la Política</h2>
      <p>
        Esta política puede ser actualizada periódicamente. Te notificaremos sobre cambios significativos en nuestra
        política de privacidad.
      </p>
    </StaticPageLayout>
  );
}
