#!/bin/bash

# if $BlocksQuiet=true then notification will not show on console
if [ -z $BlocksQuiet ]; then
    echo $1
fi

username=$(cat /root/.multichain/${MC_CHAIN_NAME}/multichain.conf | grep rpcuser | cut -d '=' -f2)
password=$(cat /root/.multichain/${MC_CHAIN_NAME}/multichain.conf | grep rpcpassword | cut -d '=' -f2)
curl -s -u $username:$password -X POST $ReceivingBlockNotify -H 'Content-Type:application/json' -d ''\{"block":\"$1\"\}''

#curl -s -X POST $ReceivingBlockNotify -H 'Content-Type:application/json' -d ''\{"block":\"$1\"\}''



