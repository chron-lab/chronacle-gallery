if (Test-Path -Path .env){
    echo "WARNING:"
    echo "The file '.env' already exists."
    exit(0)
}

docker-compose pull
docker-compose -p c8c-gallery-testnet up -d
Start-Sleep -Seconds 10

echo "MC_RPC_USER=multichainrpc" > .env
(docker exec -it mc-localnode-testnet cat /root/.multichain/chronnet/multichain.conf | findstr rpcpassword) -match 'rpcpassword=(?<P>.+)$'
echo "MC_RPC_PASSWORD=$($Matches.P)" >> .env

docker-compose -p c8c-gallery-testnet down
