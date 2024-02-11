
export interface Player {
    tag: string
    name: string
    townHallLevel: number
    townHallWeaponLevel: number
    expLevel: number
    trophies: number
    bestTrophies: number
    warStars: number
    attackWins: number
    defenseWins: number
    builderHallLevel: number
    builderBaseTrophies: number
    bestBuilderBaseTrophies: number
    role: string
    warPreference: string
    donations: number
    donationsReceived: number
    clanCapitalContributions: number
    clan: Clan
    league: League
    builderBaseLeague: BuilderBaseLeague
    legendStatistics: LegendStatistics
    achievements: Achievement[]
    playerHouse: PlayerHouse
    labels: Label[]
    troops: Troop[]
    heroes: Hero[]
    heroEquipment: HeroEquipment[]
    spells: Spell[]
}
export interface Label {
    id: number
    name: string
    iconUrls: IconUrls
}
export interface Clan {
    tag: string
    name: string
    clanLevel: number
    badgeUrls: BadgeUrls
}

export interface BadgeUrls {
    small: string
    large: string
    medium: string
}

export interface League {
    id: number
    name: string
    iconUrls: IconUrls
}

export interface IconUrls {
    small: string
    tiny: string
    medium: string
}

export interface BuilderBaseLeague {
    id: number
    name: string
}

export interface LegendStatistics {
    legendTrophies: number
    previousSeason: PreviousSeason
    bestSeason: BestSeason
    currentSeason: CurrentSeason
}

export interface PreviousSeason {
    id: string
    rank: number
    trophies: number
}

export interface BestSeason {
    id: string
    rank: number
    trophies: number
}

export interface CurrentSeason {
    rank: number
    trophies: number
}

export interface Achievement {
    name: string
    stars: number
    value: number
    target: number
    info: string
    completionInfo?: string
    village: string
}

export interface PlayerHouse {
    elements: Element[]
}

export interface Element {
    id: number
    type: string
}

export interface Troop {
    name: string
    level: number
    maxLevel: number
    village: string
    superTroopIsActive: boolean
}

export interface Hero {
    name: string
    level: number
    maxLevel: number
    equipment?: Equipment[]
    village: string
}

export interface Equipment {
    name: string
    level: number
    maxLevel: number
    village: string
}

export interface HeroEquipment {
    name: string
    level: number
    maxLevel: number
    village: string
}

export interface Spell {
    name: string
    level: number
    maxLevel: number
    village: string
}
