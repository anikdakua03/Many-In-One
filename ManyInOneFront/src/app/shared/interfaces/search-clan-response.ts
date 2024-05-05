export interface ISearchClanResponse {
    succeed: boolean
    message: any
    errors: any
    result: IResultClan[]
}

export interface IResultClan {
    tag: string
    name: string
    type: string
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
    labels: ILabel[]
    requiredBuilderBaseTrophies: number
    requiredTownhallLevel: number
    chatLanguage: IChatLanguage | null
}

export interface ILocation {
    countryCode?: string | null
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

export interface ILabel {
    id: number
    name: string
    iconUrls: IIconUrls
}

export interface IIconUrls {
    small: string
    medium: string
}

export interface IChatLanguage {
    id: number
    name: string
    languageCode: string
}
