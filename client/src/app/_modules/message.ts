export interface Message {
    id: number
    senderId: number
    recipientId: number
    recipientPhotoUrl: string
    senderUsername: string
    senderPhotoUrl: string
    recipientUsername: string
    content: string
    dateRead?: Date
    messageSent: Date
  }
  