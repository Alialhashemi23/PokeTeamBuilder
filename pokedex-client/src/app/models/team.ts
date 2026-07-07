import { Pokemon } from './pokemon';

export interface TeamMember {
    teamPokemonId: number;
    pokemon: Pokemon;
}

export interface Team {
    id: number;
    name: string;
    createdDate: string;
    members: TeamMember[];
}
