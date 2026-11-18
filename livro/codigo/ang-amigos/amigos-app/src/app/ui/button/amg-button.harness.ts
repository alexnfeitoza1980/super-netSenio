import { ComponentHarness } from '@angular/cdk/testing';

export class AmgButtonHarness extends ComponentHarness {
  static hostSelector = 'amg-button';

  private locateButton = this.locatorFor('button');

  async getText(): Promise<string> {
    const btn = await this.locateButton();
    return btn.text();
  }

  async isDisabled(): Promise<boolean> {
    const btn = await this.locateButton();
    return (await btn.getAttribute('disabled')) !== null;
  }

  async click(): Promise<void> {
    const btn = await this.locateButton();
    return btn.click();
  }
}

