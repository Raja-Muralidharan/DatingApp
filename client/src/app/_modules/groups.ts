export interface Group {

    name: string;
    connections: connection[]
}

export interface connection {   
    connectionId: string;
    username: string;
}