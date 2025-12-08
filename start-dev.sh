#!/bin/bash
# Запуск в режиме разработки

echo "  Stopping existing containers..."
docker-compose down

echo "  Starting PostgreSQL and Adminer..."
docker-compose up -d postgres adminer

echo "  Waiting for PostgreSQL to be ready..."
sleep 15

echo "  Applying EF Core migrations..."
dotnet ef database update

echo "   Development environment is ready!"
echo "   PostgreSQL: localhost:5432"
echo "   Adminer: http://localhost:8080"
echo "   Database: journal_db"
echo "   User: dev_user"
