#!/bin/bash

if [ ! -d "/root/.multichain/${MC_CHAIN_NAME}" ]; then
	echo MULTICHAIN NOT INSTALLED. CREATING MULTICHAIN...
	# chain is not created

	if [[ -n "${MC_PTEKEY}" ]]; then
		# private key is provided
		multichaind -initprivkey=${MC_PTEKEY} ${MC_CHAIN_NAME}@${MC_SEED_IP}:${MC_NETWORK_PORT} -explorersupport=2 -autosubscribe='assets,streams' -daemon
	else
		# private key is not provided
		multichaind ${MC_CHAIN_NAME}@${MC_SEED_IP}:${MC_NETWORK_PORT} -explorersupport=2 -autosubscribe='assets,streams' -daemon
	fi
	# wait until process is started (ie. pid is created)
	while [[ ! -e /root/.multichain/${MC_CHAIN_NAME}/multichain.pid ]]; do sleep 1s; done

	# shutdown the process
	echo RESTARTING MULTICHAIND...
	sleep 5
	pkill multichaind
	sleep 5

	# wait until process is terminated (ie. pid is removed)
	while [[ -e /root/.multichain/${MC_CHAIN_NAME}/multichain.pid ]]; do sleep 1s; done

fi

# configure api
cd /root/.multichain/${MC_CHAIN_NAME}
if [[ -n "${MC_RPC_PASSWORD}" ]]; then
	echo USE EXISTING RPCPASSWORD
	sed -i "s/rpcpassword.*$/rpcpassword=${MC_RPC_PASSWORD}/" multichain.conf
fi
if [[ -n "${MC_RPC_USER}" ]]; then
	echo USE EXISTING RPCUSER
	sed -i "s/rpcuser.*$/rpcuser=${MC_RPC_USER}/" multichain.conf
fi
sed -i "/rpcallowip=.*$/d" multichain.conf
echo "" >> multichain.conf

if [[ -n "${MC_RPC_ALLOWIP}" ]]; then
	echo "rpcallowip=${MC_RPC_ALLOWIP}" >> multichain.conf
	echo Allow RPC at ${MC_RPC_ALLOWIP}
else
	echo "rpcallowip=172.35.0.100" >> multichain.conf
	echo Allow RPC at 172.35.0.100
fi

# configure listeners
sed -i "/walletnotify=.*$/d" multichain.conf
echo "walletnotify=/root/notify.sh %j %n" >> multichain.conf
sed -i "/blocknotify=.*$/d" multichain.conf
echo "blocknotify=/root/block-notify.sh %s" >> multichain.conf

# Configure and start explorer
cd /root/multichain-explorer-2
sed -i "s/chain1/${MC_CHAIN_NAME}/" /root/multichain-explorer-2/config.ini
if [[ -n "${MC_EXPLORER_PORT}" ]]; then
	sed -i "s/port=.*$/port=${MC_EXPLORER_PORT}/" /root/multichain-explorer-2/config.ini
fi
python3 -m explorer config.ini daemon

# Startup
echo Starting local node..
multichaind ${MC_CHAIN_NAME} -reindex -rescan -lockinlinemetadata=0 -autosubscribe='assets,streams'
