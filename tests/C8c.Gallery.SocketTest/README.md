# Socket Test

Check web socket connection with local API server at http://localhost:12038/transaction.

NOTE: Do not run in ps and not wsl otherwise will require portproxy.

1. On one terminal, run this.

```sh
dotnet run
```

2. On another terminal, Send an empty transaction.

```sh
docker exec -it mc-localnode multichain-cli chronnet send <walletaddress> 0
```

3. If socket is working, the first terminal should show transaction. If it only show blocks but not transactions, that means the IP has been tampered with.
