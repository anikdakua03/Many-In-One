export class SearchClansRequest{
    name? : string
    warFrequency? : string
    locationId?: number
    minMembers? : number
    maxMembers? : number
    minClanPoints? : number
    minClanLevel? : number
    labels? : string[] = []
    limit? : number
}

// Problem is if i do not select min members locations then it is send null as replace ment to sever , which server will not accept , it s normal , so need to check  or preprapre like that way