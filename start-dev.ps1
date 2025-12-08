# Запуск в режиме разработки

Write-Host "  Stopping existing containers..." -ForegroundColor Yellow
docker-compose down

Write-Host "  Starting PostgreSQL and Adminer..." -ForegroundColor Green
docker-compose up -d postgres adminer

Write-Host "  Waiting for PostgreSQL to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

Write-Host "  Applying EF Core migrations..." -ForegroundColor Cyan
dotnet ef database update

Write-Host "   Development environment is ready!" -ForegroundColor Green
Write-Host "   PostgreSQL: localhost:5432"
Write-Host "   Adminer: http://localhost:8080"
Write-Host "   Database: journal_db"
Write-Host "   User: dev_user"
