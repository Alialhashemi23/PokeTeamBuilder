import { PokemonType } from "./pokemon-type";

export interface Pokemon {
    id: number;
    pokedexNumber: number;
    name: string;
    hp: number;
    attack: number;
    defense: number;
    specialAttack: number;
    specialDefense: number;
    speed: number;
    primaryType: PokemonType;
    secondaryType?: PokemonType;
}
