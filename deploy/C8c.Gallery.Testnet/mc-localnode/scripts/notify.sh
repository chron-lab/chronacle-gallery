#!/bin/bash

# if $TxQuiet=true then notification will not show on console
if [ -z $TxQuiet ]; then
    echo $1
fi

# publish only if mined (ie. height != -1)
if [ $2 -ne "-1" ]; then
    username=$(cat /root/.multichain/${MC_CHAIN_NAME}/multichain.conf | grep rpcuser | cut -d '=' -f2)
    password=$(cat /root/.multichain/${MC_CHAIN_NAME}/multichain.conf | grep rpcpassword | cut -d '=' -f2)
    curl -s -u $username:$password -X POST $ReceivingHost -H 'Content-Type:application/json' -d ''\{"txn":"$1","height":$2\}''
fi
