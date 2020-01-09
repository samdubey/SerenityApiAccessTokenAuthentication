namespace SereneApp.Northwind {
    export enum OrderShippingState {
        NotShipped = 0,
        Shipped = 1
    }
    Serenity.Decorators.registerEnumType(OrderShippingState, 'SereneApp.Northwind.OrderShippingState', 'Northwind.OrderShippingState');
}
