import { createApp } from 'https://unpkg.com/vue@3/dist/vue.esm-browser.js';

const App = {
  data() {
    return { products: [] };
  },
  async mounted() {
    const res = await fetch('/api/product');
    this.products = await res.json();
  },
  template: `
    <div>
      <h1>Product List</h1>
      <ul>
        <li v-for="p in products" :key="p.id">
          {{ p.name }} - ${{ p.price }}
        </li>
      </ul>
    </div>
  `
};

createApp(App).mount('#app');