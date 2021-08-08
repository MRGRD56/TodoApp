export default class Checkable<T> {
    public isChecked: boolean;
    public item: T;

    constructor(item: T, isChecked: boolean = false) {
        this.item = item;
        this.isChecked = isChecked;
    }
}
