export interface IClanInfoResponse {
    succeed: boolean
    message: any
    errors: any
    result: IClanInfo
}

export interface IClanInfo {
    tag: string
    name: string
    type: string
    description: string
    location: ILocation
    isFamilyFriendly: boolean
    badgeUrls: IBadgeUrls
    clanLevel: number
    clanPoints: number
    clanBuilderBasePoints: number
    clanCapitalPoints: number
    capitalLeague: ICapitalLeague
    requiredTrophies: number
    warFrequency: string
    warWinStreak: number
    warWins: number
    warTies: number
    warLosses: number
    isWarLogPublic: boolean
    warLeague: IWarLeague
    members: number
    memberList: IMemberList[]
    labels: ILabel[]
    requiredBuilderBaseTrophies: number
    requiredTownhallLevel: number
    clanCapital: IClanCapital
}

export interface ILocation {
    id: number
    name: string
    isCountry: boolean
}

export interface IBadgeUrls {
    small: string
    large: string
    medium: string
}

export interface ICapitalLeague {
    id: number
    name: string
}

export interface IWarLeague {
    id: number
    name: string
}

export interface IMemberList {
    tag: string
    name: string
    role: string
    townHallLevel: number
    expLevel: number
    league: ILeague
    trophies: number
    builderBaseTrophies: number
    clanRank: number
    previousClanRank: number
    donations: number
    donationsReceived: number
    playerHouse?: IPlayerHouse
    builderBaseLeague: IBuilderBaseLeague
}

export interface ILeague {
    id: number
    name: string
    iconUrls: IIconUrls
}

export interface IIconUrls {
    small?: string
    tiny?: string
    medium?: string
}

export interface IPlayerHouse {
    elements: IElement[]
}

export interface IElement {
    id: number
    type: string
}

export interface IBuilderBaseLeague {
    id: number
    name: string
}

export interface ILabel {
    id: number
    name: string
    iconUrls?: IIconUrls
}


export interface IClanCapital {
    capitalHallLevel: number
    districts: IDistrict[]
}

export interface IDistrict {
    id: number
    name: string
    districtHallLevel: number
}
