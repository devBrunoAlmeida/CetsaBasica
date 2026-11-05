#!/usr/bin/env bash
set -e

cd "$(dirname "$0")"

echo "1/3 Subindo banco PostgreSQL de teste (Docker)..."
docker compose up -d

# Garante que os containers da Evolution API (Redis + Postgres + API), se existirem na máquina, estejam ativos
for cname in evolution-postgres evolution-redis evolution-api; do
    if docker container inspect "$cname" >/dev/null 2>&1; then
        docker start "$cname" >/dev/null 2>&1 || true
    fi
done

echo "2/3 Aguardando PostgreSQL aceitar conexões..."
for i in {1..15}; do
    if docker exec postgres-cestabasica pg_isready -U postgres -d cestabasica_teste >/dev/null 2>&1; then
        break
    fi
    sleep 1
done

cleanup() {
    echo ""
    echo "Encerrando API e Web..."
    kill "$API_PID" "$WEB_PID" 2>/dev/null || true
    wait "$API_PID" "$WEB_PID" 2>/dev/null || true
}
trap cleanup EXIT INT TERM

echo "3/3 Iniciando CestaBasica.Api (porta 5237) e CestaBasica.Web (porta 5093)..."
dotnet run --project CestaBasica.Api/CestaBasica.Api.csproj --launch-profile http &
API_PID=$!

dotnet run --project CestaBasica.Web/CestaBasica.Web.csproj --launch-profile http &
WEB_PID=$!

echo ""
echo "=========================================================="
echo " Sistema no ar!"
echo " -> Web (Login): http://localhost:5093/login"
echo " -> API Swagger: http://localhost:5237/swagger"
echo " Pressione Ctrl+C para parar API e Web."
echo "=========================================================="

wait
