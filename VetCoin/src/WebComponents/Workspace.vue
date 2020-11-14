<template>
    <div>
        12123444 
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';


    @Component({
        components: {
        },
    })
    export default class Template extends Vue {
        @Prop() public value: number;

        get localValue() { return this.value; }
        set localValue(value: number) {
            this.$emit('input', value);
        }


    }
</script>

<style>

    @import 'https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css';

    .btn {
        margin-left: 5px;
    }
</style>
