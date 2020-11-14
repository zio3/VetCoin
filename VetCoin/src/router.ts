import Vue from 'vue';
import Router from 'vue-router';

Vue.use(Router);

export default new Router({
    routes: [
    {
        path: '/TradeLikeVote',
        name: 'TradeLikeVote',
        component: () => import('./WebComponents/TradeLikeVote.vue')
        },
    {
        path: '/Workspace',
        name: 'Workspace',
        component: () => import('./WebComponents/Workspace.vue')
        },
]
});
