namespace SerebiiParser.Core.UnitTests

open Xunit
open SerebiiParser.Core

module PokemonParserTests =
    
    [<Fact>]
    let ``Should parse Charizard from Serebii``() =
        
        let expected = 
           {number = 6;
            name = "Charizard";
            classification = "Flame";
            type1 = Fire;
            type2 = Some Flying;
            height = 1.7M;
            weight = 90.5M;
            captureRate = 45;
            baseEggSteps = 5120;
            maleGenderRatio = Some 87.5M;
            experienceGrowth = 1059860;
            baseHappiness = 70;
            hp = 78;
            attack = 84;
            defense = 78;
            spAttack = 109;
            spDefense = 85;
            speed = 100;
            ability1 = "blaze";
            ability2 = None
            hiddenAbility = Some "solar_power"}

        let actual = PokemonParser.parsePokemon 6

        Assert.True((expected = actual))

    [<Fact>]
    let ``Should parse Metapod from Serebii``() =
        
        let expected = 
           {number = 11;
            name = "Metapod";
            classification = "Cocoon";
            type1 = Bug;
            type2 = None;
            height = 0.7M;
            weight = 9.9M;
            captureRate = 120;
            baseEggSteps = 3840;
            maleGenderRatio = Some 50M;
            experienceGrowth = 1000000;
            baseHappiness = 70;
            hp = 50;
            attack = 20;
            defense = 55;
            spAttack = 25;
            spDefense = 25;
            speed = 30;
            ability1 = "shed_skin";
            ability2 = None
            hiddenAbility = None}

        let actual = PokemonParser.parsePokemon 11

        Assert.True((expected = actual))

    [<Fact>]
    let ``Should parse Rattata from Serebii``() =
        
        let expected = 
           {number = 19;
            name = "Rattata";
            classification = "Mouse";
            type1 = Normal;
            type2 = None;
            height = 0.3M;
            weight = 3.5M;
            captureRate = 255;
            baseEggSteps = 3840;
            maleGenderRatio = Some 50M;
            experienceGrowth = 1000000;
            baseHappiness = 70;
            hp = 30;
            attack = 56;
            defense = 35;
            spAttack = 25;
            spDefense = 35;
            speed = 72;
            ability1 = "run_away";
            ability2 = Some "guts"
            hiddenAbility = Some "hustle"}

        let actual = PokemonParser.parsePokemon 19

        Assert.True((expected = actual))