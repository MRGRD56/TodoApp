export default interface TodoItem {
    id: number;
    userId: number;
    creationTime: string;
    text: string;
    isDone: boolean;
    isDeleted: boolean;
}
