import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-horizontal-loading',
    templateUrl: './horizontal-loading.component.html',
    styleUrls: ['./horizontal-loading.component.scss']
})
export class HorizontalLoadingComponent implements OnInit {

    public width: string = "100px";

    constructor() {
    }

    ngOnInit() {
    }

}
