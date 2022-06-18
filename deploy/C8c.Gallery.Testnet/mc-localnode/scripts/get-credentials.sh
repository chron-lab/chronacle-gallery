cat /root/.multichain/${MC_CHAIN_NAME}/multichain.conf
username=$(cat .multichain/${MC_CHAIN_NAME}/multichain.conf | grep rpcuser | cut -d '=' -f2)
password=$(cat .multichain/${MC_CHAIN_NAME}/multichain.conf | grep rpcpassword | cut -d '=' -f2)