<template>
    <div>
        <h2>Template</h2>
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

    public created() {

    }
}
</script>

<style>
</style>
