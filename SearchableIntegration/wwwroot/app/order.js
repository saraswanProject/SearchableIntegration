const OrderList = {
    template: '#order-template',
    data() {
        return {
            orders: [],
            newOrder: {
                customerName: '',
                customerEmail: '',
                orderDate: new Date().toISOString().slice(0, 10),
                orderStatus: 'Pending',
                totalAmount: 0,
                orderDetails: []
            },
            products: [],
            editId: null,
            currentPage: 1,
            pageSize: 5
        };
    },
    computed: {
        pagedOrders() {
            const start = (this.currentPage - 1) * this.pageSize;
            return this.orders.slice(start, start + this.pageSize);
        },
        totalPages() {
            return Math.ceil(this.orders.length / this.pageSize);
        }
    },
    methods: {
        async loadOrders() {
            debugger
            const res = await fetch('/api/orderapi/getorders');
            this.orders = await res.json();
        },
        async loadProducts() {
            const res = await fetch('/api/orderapi/getproducts');
            this.products = await res.json();
        },
        addOrderDetail() {
            this.newOrder.orderDetails.push({ productID: '', quantity: 1, subTotal: 0 });
        },
        calculateTotal() {
            this.newOrder.totalAmount = this.newOrder.orderDetails.reduce((sum, item) => sum + item.subTotal, 0);
        },
        updateSubTotal(item) {
            debugger
            const product = this.products.find(p => p.id === item.productID);
            const price = product ? product.price : 0;
            item.price = product.price;
            item.subTotal = item.quantity * price;
            this.calculateTotal();
        },
        async addOrder() {
            this.calculateTotal();
            const res = await fetch('/api/orderapi/create', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(this.newOrder)
            });
            const result = await res.json();
            alert(result.message || 'Order created.');
            this.resetForm();
            await this.loadOrders();
        },
        async editOrder(order) {
            this.editId = order.orderID;
            const res = await fetch(`/api/orderapi/getorderdetails/${order.orderID}`, { method: 'GET' });
            const details = await res.json();
            const enrichedDetails = details.map(d => {
                const product = this.products.find(p => p.id === d.ProductID);
                const price = product ? product.price : 0;
                return {
                    orderID: d.OrderID,
                    productID: d.ProductID,
                    quantity: d.Quantity,
                    price: price,
                    subTotal: d.Quantity * price
                };
            });
            this.newOrder = {
                orderID: order.orderID, // Make sure this is set
                customerName: order.customerName,
                customerEmail: order.customerEmail,
                orderDate: order.orderDate,
                orderStatus: order.orderStatus,
                totalAmount: order.totalAmount,
                orderDetails: enrichedDetails
            };
        },
        async updateOrder() {
            this.calculateTotal();
            const res = await fetch('/api/orderapi/update', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(this.newOrder)
            });
            const result = await res.json();
            alert(result.message || 'Order updated.');
            this.resetForm();
            await this.loadOrders();
        },
        async deleteOrder(id) {
            if (!confirm('Delete this order?')) return;
            const res = await fetch(`/api/orderapi/delete/${id}`, { method: 'DELETE' });
            const result = await res.json();
            alert(result.message || 'Deleted.');
            await this.loadOrders();
        },
        resetForm() {
            this.newOrder = {
                customerName: '',
                customerEmail: '',
                orderDate: new Date().toISOString().slice(0, 10),
                orderStatus: 'Pending',
                totalAmount: 0,
                orderDetails: []
            };
            this.editId = null;
        },
        goToPage(page) {
            if (page >= 1 && page <= this.totalPages) {
                this.currentPage = page;
            }
        }
    },
    async mounted() {
        await this.loadProducts();
        await this.loadOrders();
    }
};

const app = Vue.createApp({});
app.component('order-list', OrderList);
app.mount('#order-app');
