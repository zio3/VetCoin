<template>
    <div @click="toggle()">
        <i class="fas fa-thumbs-up" :class="state" onclick=""></i>
        <span class="numcount">{{voteCount}}</span>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from 'vue-property-decorator';
import { ApiException, TradeLikeVotesClient } from '../api/vetcoin';
import CommonConfiguration from '../Codes/CommonConfigration';

@Component({
    components: {
    },
})
export default class Template extends Vue {

    @Prop() tradeId: number;
    @Prop() voteCount: number;
    @Prop() isVoted: boolean;

    tradeLikeVotesClient: TradeLikeVotesClient;

    currentState: boolean = true;

    state = {
        off: true,
        on: false,
    };

    public created() {
        const baseUrl = CommonConfiguration.getBaseUrl();
        this.tradeLikeVotesClient = new TradeLikeVotesClient(baseUrl);
    }

    async toggle() {
        const responce = await this.tradeLikeVotesClient.postTradeLikeVote(this.tradeId);

        this.currentState = responce.isVoted;
        this.voteCount = responce.count;

        this.state.off = !this.currentState;
        this.state.on = this.currentState;
    }


}
</script>

<style>

    @import 'https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css';

    .off {
        color: #CCCCFF
    }

    .on {
        color: #A0A0E0
    }
    .numcount {
        color: "#E0E0E0";
        margin-left: 5px;
    }

    .btn {
        margin-left: 5px;
    }
</style>
