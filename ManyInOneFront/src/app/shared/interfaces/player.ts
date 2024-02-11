export interface IPlayerResponse {
    succeed: boolean;
    message: any;
    errors: any;
    result: IPlayer;
}

export interface IPlayer {
    tag: string;
    name: string;
    townHallLevel: number;
    townHallWeaponLevel: number;
    expLevel: number;
    trophies: number;
    bestTrophies: number;
    warStars: number;
    attackWins: number;
    defenseWins: number;
    builderHallLevel: number;
    builderBaseTrophies: number;
    bestBuilderBaseTrophies: number;
    role: string;
    warPreference: string;
    donations: number;
    donationsReceived: number;
    clanCapitalContributions: number;
    clan: IClan;
    league: ILeague;
    builderBaseLeague: IBuilderBaseLeague;
    legendStatistics: ILegendStatistics;
    achievements: IAchievement[];
    playerHouse: IPlayerHouse;
    labels: ILabel[];
    troops: ITroop[];
    heroes: IHero[];
    heroEquipment: IHeroEquipment[];
    spells: ISpell[];
}
export interface ILabel {
    id: number;
    name: string;
    iconUrls: IIconUrls;
}
export interface IClan {
    tag: string;
    name: string;
    clanLevel: number;
    badgeUrls: IBadgeUrls;
}

export interface IBadgeUrls {
    small: string;
    large: string;
    medium: string;
}

export interface ILeague {
    id: number;
    name: string;
    iconUrls: IIconUrls;
}

export interface IIconUrls {
    small: string;
    tiny: string;
    medium: string;
}

export interface IBuilderBaseLeague {
    id: number;
    name: string;
}

export interface ILegendStatistics {
    legendTrophies: number;
    previousSeason: IPreviousSeason;
    bestSeason: IBestSeason;
    currentSeason: IICurrentSeason;
}

export interface IPreviousSeason {
    id: string;
    rank: number;
    trophies: number;
}

export interface IBestSeason {
    id: string;
    rank: number;
    trophies: number;
}

export interface IICurrentSeason {
    rank: number;
    trophies: number;
}

export interface IAchievement {
    name: string;
    stars: number;
    value: number;
    target: number;
    info: string;
    completionInfo?: string;
    village: string;
}

export interface IPlayerHouse {
    elements: IElement[];
}

export interface IElement {
    id: number;
    type: string;
}

export interface ITroop {
    name: string;
    level: number;
    maxLevel: number;
    village: string;
    superTroopIsActive: boolean;
}

export interface IHero {
    name: string;
    level: number;
    maxLevel: number;
    equipment?: IEquipment[];
    village: string;
}

export interface IEquipment {
    name: string;
    level: number;
    maxLevel: number;
    village: string;
}

export interface IHeroEquipment {
    name: string;
    level: number;
    maxLevel: number;
    village: string;
}

export interface ISpell {
    name: string;
    level: number;
    maxLevel: number;
    village: string;
}
