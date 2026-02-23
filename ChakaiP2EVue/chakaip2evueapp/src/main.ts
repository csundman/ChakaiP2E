import './assets/main.css';

import { createApp } from 'vue';
import App from './App.vue';
import router from './router/router.ts';

const app = createApp(App);

// Auto-register all .vue components in the components directory
const components = import.meta.glob('./components/common/*.vue', { eager: true });

for (const path in components) {
    const component: any = components[path];
    const name = component.default.name || path.split('/').pop()?.replace('.vue', '');
    if (name) {
        app.component(name, component.default);
    }
}

app.use(router).mount('#app');
