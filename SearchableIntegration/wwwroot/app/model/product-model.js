export default class Product {
    constructor({
        id,
        name,
        category,
        price,
        stockQuantity,
        supplierID,
        manufacturedDate,
        expiryDate
    }) {
        this.id = id;
        this.name = name;
        this.category = category;
        this.price = price;
        this.stockQuantity = stockQuantity;
        this.supplierID = supplierID;
        this.manufacturedDate = manufacturedDate;
        this.expiryDate = expiryDate;
    }
}
