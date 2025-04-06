import { createApp } from 'vue';
import App from './App.vue';
import { createI18n } from 'vue-i18n';

import en from './locales/en.json';
import no from './locales/no.json';
import sv from './locales/sv.json';

const i18n = createI18n({
    legacy: false,
    globalInjection: true,
    locale: 'en',
    fallbackLocale: 'en',
    messages: {
        en,
        no,
        sv
    }
});

const app = createApp(App);
app.use(i18n);
app.mount('#app');
