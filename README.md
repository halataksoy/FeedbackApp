# Feedback App

Basit bir kullanıcı geri bildirim uygulamasıdır. Kullanıcılar frontend üzerinden form doldurarak geri bildirim gönderebilir. Gönderilen veriler ASP.NET Core 8 Web API tarafından RabbitMQ kuyruğuna iletilir ve RabbitMQ consumer servisi tarafından MongoDB'ye kaydedilir.
![image](https://github.com/user-attachments/assets/407bf7aa-a0cc-4de9-90d9-3e40014cf086)
![image](https://github.com/user-attachments/assets/72d4d1b4-ab65-4991-8485-deed7455610b)
![image](https://github.com/user-attachments/assets/53bb647f-80cc-4e17-b536-2410181ee395)



---

## Teknolojiler

- Backend: ASP.NET Core 8 Web API  
- Frontend: ReactJS (Vite)  
- Mesaj Kuyruğu: RabbitMQ  
- Veritabanı: MongoDB  
- Docker Compose ile RabbitMQ ve MongoDB servisleri çalıştırılır.

---

## Proje Yapısı

- **Backend**: `/Backend/FeedbackApi`  
- **Worker (RabbitMQ Consumer)**: `/Backend/FeedbackWorker`  
- **Frontend**: `/Frontend/feedback-app`

---

## Çalıştırma

### Gerekli araçlar

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  
- [Node.js](https://nodejs.org/) (frontend için)  
- [Docker & Docker Compose](https://www.docker.com/get-started)

### Adımlar

1. Docker ile RabbitMQ ve MongoDB servislerini çalıştırın:
   ```bash
   docker-compose up -d
   
cd Backend/FeedbackApi
dotnet run


cd Frontend/feedback-app
npm install
npm run dev
