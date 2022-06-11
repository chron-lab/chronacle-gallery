$yes= Read-Host -Prompt "Warning: This will uninstall ChronAcle Gallery entirely. Make sure you have your private key backed-up. Press Y to proceed or any key to terminate"
if ($Yes -eq "Y") {
    echo "Shutting down services"
    docker-compose down
    echo "Delete config"
    rm .env
    echo "Removing volume"
    docker volume rm testnet-user_mc-localnode-vol
    echo "ChronAcle Gallery is uninstalled."
}
