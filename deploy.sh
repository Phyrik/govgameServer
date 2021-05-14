cd /home/pi/govgameServer
git reset --hard origin/master
git pull
dotnet run secure 5001 --project govgameGameServer --configuration Release