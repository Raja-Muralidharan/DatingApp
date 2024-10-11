import { Photo } from "./Photo"

export interface Member {
    id: number
    userName: string
    age: number
    photoURL: string
    knownAs: string
    created: Date
    lastActive: Date
    gender: string
    introduction: string
    interests: string
    lookingFor: string
    city: string
    country: string
    photos: Photo[]
  }
  
