import { createApp } from 'https://unpkg.com/vue@3/dist/vue.esm-browser.js';

const login = {
    template: '#login-template',
  
        data() {
            return {
                credentials: {
                    email: '',
                    password: ''
                    },
                error: ''
                };
            },
    methods: {
        async submitLogin() {
            console.log("Login initiated");
            const payload = {
                email: this.credentials.email,
                password: this.credentials.password
            };

            try {
                console.log("Login Payload:", payload);

                const res = await fetch('/api/accountapi/login', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Accept': 'application/json'
                    },
                    body: JSON.stringify(payload)
                });

                if (!res.ok) {
                    const errorText = await res.text();
                    console.error("Login API Error Response:", errorText);
                    throw new Error(errorText || 'Login failed');
                }

                const response = await res.json();
                console.log("Login successful:", response);
                location.href = '/home/index'; // Redirect after login

            } catch (error) {
                console.error('Error during login:', error);
                this.error = error.message || 'Login failed';
                alert(this.error);
            }
        }
    }
};
    

const app = createApp({});
app.component('login', login);
app.mount('#login-app');
