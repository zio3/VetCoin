<template>
    <div>
        <AppRoot :tradeId="localTradeId" :voteCount="localVoteCount" :isVoted="localIsVotedt" ></AppRoot>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import AppRoot from '../Components/TradeLikeVote.vue';

    @Component({
        components: {
            AppRoot
        },
    })
    export default class Template extends Vue {
        @Prop() tradeId: number;
        @Prop() voteCount: number;
        @Prop() isVoted: boolean;


        devTradeId: number = 172;
        devVoteCount: number = 22;
        devIsVoted: boolean = false;

        get localTradeId() {
            if (this.isDevMode) {
                return this.devTradeId;
            } else {
                return this.tradeId;
            }
        }
        

        get localVoteCount() {
            if (this.isDevMode) {
                return this.devVoteCount;
            } else {
                return this.voteCount;
            }
        }
        set localVoteCount(value) {
            if (this.isDevMode) {
                this.devVoteCount = value;
            } else {
                this.voteCount = value;
            }
        }

        get localIsVotedt() {
            if (this.isDevMode) {
                return this.devIsVoted;
            } else {
                return this.isVoted;
            }
        }


        get isDevMode() {
            return this.$attrs.mode == "dev";
        }

        public created() {
        }
    }
</script>

<style>


    .btn {
        margin-left: 5px;
    }
</style>
