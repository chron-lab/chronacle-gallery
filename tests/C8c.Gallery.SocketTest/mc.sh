echo getinfo
echo -------
curl --user multichainrpc:C6n97oxTJrEqmwvVrWGP5TgHpyewRjz2x3soDQKLFkWq --data-binary '{"jsonrpc": "1.0", "id":"curltest", "method": "getinfo", "params": [] }' -H 'content-type: text/plain;' http://127.0.0.1:12011
echo
echo send 12S7Eg2Gz1ZSdRXqVjzjoSybBV1m9umdZz5nHL 0
echo ---------------------------------------------
curl --user multichainrpc:C6n97oxTJrEqmwvVrWGP5TgHpyewRjz2x3soDQKLFkWq --data-binary '{"jsonrpc": "1.0", "id":"curltest", "method": "send", "params": ["12S7Eg2Gz1ZSdRXqVjzjoSybBV1m9umdZz5nHL",1000] }' -H 'content-type: text/plain;' http://127.0.0.1:12011