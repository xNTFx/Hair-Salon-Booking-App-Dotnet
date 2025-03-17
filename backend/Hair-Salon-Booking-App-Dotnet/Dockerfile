FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /app

COPY . ./
RUN dotnet restore

RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

COPY --from=build /app/out .

RUN apt-get update && apt-get install -y postgresql-client && rm -rf /var/lib/apt/lists/*

COPY Database /Database
COPY Database/hair-salon-booking-app-database-plain.sql /Database/hair-salon-booking-app-database-plain.sql

EXPOSE 8080

ENV ASPNETCORE_URLS="http://+:8080"

CMD ["sh", "-c", "until pg_isready -h db -p 5432 -U postgres; do sleep 2; done && PGPASSWORD=password psql -h db -U postgres -d hair_salon_booking -f /Database/hair-salon-booking-app-database-plain.sql && dotnet /app/Hair_Salon_Booking_App_Dotnet.dll"]
