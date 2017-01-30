namespace SerebiiParser.Core

type Pokemon = {number: int;
    name: string;
    classification: string;
    type1: PokemonType;
    type2: Option<PokemonType>;
    height: decimal;
    weight: decimal;
    captureRate: int;
    baseEggSteps: int;
    maleGenderRatio: Option<decimal>;
    experienceGrowth: int;
    baseHappiness: int;
    hp: int;
    attack: int;
    defense: int;
    spAttack: int;
    spDefense: int;
    speed: int;
    ability1: string;
    ability2: Option<string>;
    hiddenAbility: Option<string>}