# ComprobanteRD

Plataforma SaaS para **recibir, verificar y confirmar comprobantes de pago por transferencia enviados por WhatsApp**, pensada para pequeñas y medianas empresas de República Dominicana.

El cliente envía la foto de su comprobante por WhatsApp; el negocio lo revisa desde un panel web, lo confirma o rechaza, y el cliente recibe la respuesta y su recibo en PDF directamente en el chat.

---

## Problema que resuelve

Muchos negocios dominicanos cobran por transferencia bancaria y reciben los comprobantes por WhatsApp. Validarlos a mano, buscarlos entre conversaciones y emitir un recibo es lento y propenso a errores. ComprobanteRD centraliza ese proceso en un solo lugar.

## Cómo funciona

1. El cliente escribe al WhatsApp del negocio y recibe un menú.
2. Elige la opción de enviar comprobante y manda la foto (imagen o documento).
3. La API descarga el archivo desde WhatsApp Cloud API, lo guarda y crea un comprobante en estado `Pending`.
4. Un usuario del negocio abre el panel, revisa la imagen y **confirma** o **rechaza** el comprobante.
5. Al confirmar se genera un recibo (`REC-AAAAMMDD-ID`), se notifica al cliente por WhatsApp y se le envía el recibo en PDF.
6. Al rechazar, el cliente recibe un mensaje con el motivo.

## Características

- Recepción de comprobantes por WhatsApp (Cloud API) con menú interactivo
- Panel web con dashboard, directorio de clientes e historial de transacciones
- Confirmación y rechazo de comprobantes con notificación automática al cliente
- Generación de recibos en PDF y envío por WhatsApp
- **Multi-tenant**: cada empresa solo ve sus propios datos (filtrado por `CompanyId` en el token JWT)

## Tecnologías

| Capa | Tecnología |
|---|---|
| Backend | ASP.NET Core Web API (.NET 9), C# |
| Autenticación | ASP.NET Core Identity, JWT Bearer, BCrypt |
| Base de datos | SQL Server, Entity Framework Core  |
| Frontend | Angular 21, Angular Material, RxJS |
| PDF | html2pdf.js |
| Integración | WhatsApp Cloud API (Meta Graph API) |
| Documentación API | Swagger / Swashbuckle |

## Estructura del repositorio

```
ComprobanteRD/
├── ComprobanteRDAPI/              # Backend (.NET 9)
│   └── ComprobanteRDAPI/
│       ├── Controllers/           # Auth, Voucher, Customer, Receipt, WhatsAppWebhook
│       ├── Models/                # User, Company, Customer, Voucher, Receipt, SubscriptionPlan
│       ├── DTOs/
│       ├── Services/              # WhatsAppSenderService, MediaStorageService
│       ├── Data/                  # AppDbContext
│       └── Migrations/
└── ComprobanteRD-Angular/         # Frontend (Angular 21)
    └── src/app/
        ├── components/            # login, dashboard, voucher, customers, transactions, receipt-preview
        ├── services/
        ├── guards/ interceptors/
        └── models/
```

## Requisitos previos

- [.NET SDK 9](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) y npm
- Angular CLI (`npm install -g @angular/cli`)
- SQL Server (local, Express o Docker)
- Una cuenta de [Meta for Developers](https://developers.facebook.com/) con WhatsApp Cloud API configurada
- Una herramienta de túnel (por ejemplo ngrok) para exponer el webhook en desarrollo

## Configuración

| Clave | Descripción |
|---|---|
| `ConnectionStrings:DefaultConnection` | Cadena de conexión a SQL Server |
| `jwtkey` | Clave secreta para firmar los JWT (usa una cadena larga y aleatoria) |
| `Jwt:Issuer` / `Jwt:Audience` | Emisor y audiencia del token |
| `allowedOrigins` | Orígenes permitidos por CORS, separados por coma (ej. `http://localhost:4200`) |
| `WhatsAppSettings:AccessToken` | Token de acceso de WhatsApp Cloud API |
| `WhatsAppSettings:PhoneNumberId` | ID del número de teléfono de WhatsApp Business |
| `WhatsAppSettings:VerifyToken` | Token que defines tú para verificar el webhook |


## Instalación y ejecución

### Backend

```bash
cd ComprobanteRDAPI/ComprobanteRDAPI
dotnet restore
dotnet ef database update
dotnet run
```

La API queda disponible en `https://localhost:7298` y Swagger en `/swagger` (solo en desarrollo). Al iniciar, crea automáticamente los roles `Admin` y `Employee` si no existen.

### Frontend

```bash
cd ComprobanteRD-Angular
npm install
ng serve
```

La app queda en `http://localhost:4200`. La URL de la API se configura en `src/environments/environment.development.ts` (`apiURL`).

### Webhook de WhatsApp

1. Expón tu API local con un túnel (ej. `ngrok http https://localhost:7298`).
2. En Meta for Developers, configura el webhook con la URL `https://<tu-tunel>/api/webhook/whatsapp` y el mismo `VerifyToken` que definiste.
3. Suscríbete al evento `messages`.

## Endpoints principales

| Método | Ruta | Descripción | Auth |
|---|---|---|---|
| POST | `/api/auth/register` | Registra una empresa y su usuario administrador | No |
| POST | `/api/auth/login` | Inicia sesión y devuelve el JWT | No |
| GET | `/api/voucher` | Lista los comprobantes de la empresa | Sí |
| GET | `/api/voucher/{id}` | Detalle de un comprobante | Sí |
| GET | `/api/voucher/transactions` | Historial de transacciones (aprobadas y rechazadas) | Sí |
| POST | `/api/voucher/{id}/confirm` | Confirma el comprobante y genera el recibo | Sí |
| POST | `/api/voucher/{id}/reject` | Rechaza el comprobante con un motivo | Sí |
| GET | `/api/customer` | Directorio de clientes | Sí |
| GET | `/api/receipt/{voucherId}` | Datos del recibo | Sí |
| POST | `/api/receipt/{voucherId}/pdf` | Sube el PDF del recibo y lo envía por WhatsApp | Sí |
| GET / POST | `/api/webhook/whatsapp` | Verificación y recepción de mensajes de WhatsApp | No |

## Documentación académica

Proyecto desarrollado en el **Instituto Tecnológico de las Américas (ITLA)**.

## Licencia

Distribuido bajo la licencia MIT. Consulta el archivo [LICENSE](LICENSE) para más información.

## Autor

**Jeremy** · [@JeremyRG32](https://github.com/JeremyRG32)
