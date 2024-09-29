# Good shopping - POC project

## Top level design 

https://miro.com/app/board/uXjVLZmj9fg=/?share_link_id=98842937266

## Function App (serverless) - StoreItemApp

POC Azure function application

## API Management service - ShoppingApiManagement 

## Service Bus Namespace - Savvy-saves 

Queue to handle messages sent to StoreItemApp

## Function App - BusMessageHandler

Handles messages sent to the bus