export interface MemberMultiplier {
    teamPokemonId: number;
    pokemonName: string;
    multiplier: number;
}

export interface TypeMatchup {
    type: string;
    weak: number;
    resist: number;
    immune: number;
    members: MemberMultiplier[];
}

export interface StatSummary {
    stat: string;
    average: number;
    bestName: string;
    bestValue: number;
}

export interface TeamAnalysis {
    defense: TypeMatchup[];
    threats: string[];
    unresisted: string[];
    stats: StatSummary[];
}
