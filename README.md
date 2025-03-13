# Hair-Salon-Booking-App Dotnet

## Tech Stack

- **.NET 7.0**
- **C#**
- **React**
- **TypeScript**
- **Vite**
- **PostgreSQL**
- **JWT Authentication**

## Link to Website

https://www.hairsalonbookingapp-dotnet.pawelsobon.pl/

## Example Image

![hbs](https://github.com/user-attachments/assets/c4403f19-24e1-4598-be45-a47903736ec9)

## Installation

### Prerequisites

- .NET SDK 7.0
- Node.js & npm
- PostgreSQL database instance

### Steps

1. Clone the repository:
   ```sh
   git clone https://github.com/xNTFx/hair-salon-booking-app-dotnet.git
   ```
2. Navigate to the project directory:
   ```sh
   cd hair-salon-booking-app-dotnet
   ```
3. Install backend dependencies:
   ```sh
   dotnet restore
   ```
4. Configure environment variables (see the section below).
5. Run the backend (default port: 8080 unless specified otherwise in configuration):
   ```sh
   dotnet run
   ```
6. Navigate to the frontend directory:
   ```sh
   cd client
   ```
7. Install frontend dependencies:
   ```sh
   npm install
   ```
8. Start the frontend (default Vite port: 5173):
   ```sh
   npm run dev
   ```

## Environment Variables

Create a file named `appsettings.json` and configure it with the following values:

```json
{
  "Jwt": {
    "AccessTokenExpiration": ,  // expiration time (in minutes) for access tokens
    "AccessTokenSecret": "",  // secret key used to sign access tokens
    "RefreshTokenExpiration": ,  // expiration time (in minutes) for refresh tokens
    "RefreshTokenSecret": ""  // secret key used to sign refresh tokens
  },
  "Server": {
    "Port": 8080  // port where the application server runs (default: 8080)
  },
  "ConnectionStrings": {
    "DefaultConnection": ""  // connection string for PostgreSQL database
  }
}
```

### Running the Application

- **Backend** runs on `http://localhost:8080` (unless configured otherwise in `appsettings.json`).
- **Frontend** runs on `http://localhost:5173` (default Vite port).
