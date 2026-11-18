import { ComponentHarness } from '@angular/cdk/testing';

export class AmgDialogHarness extends ComponentHarness {
  static hostSelector = 'amg-dialog';

  private locateBackdrop = this.locatorForOptional('.amg-dialog-backdrop');
  private locatePanel = this.locatorForOptional('.amg-dialog-panel');
  private locateCloseBtn = this.locatorForOptional('.amg-dialog-close');
  private locateContent = this.locatorForOptional('.amg-dialog-content');

  async isOpen(): Promise<boolean> {
    const backdrop = await this.locateBackdrop();
    return backdrop !== null;
  }

  async getContentText(): Promise<string> {
    const content = await this.locateContent();
    return content ? content.text() : '';
  }

  async clickCloseButton(): Promise<void> {
    const btn = await this.locateCloseBtn();
    if (btn) await btn.click();
  }

  async clickBackdrop(): Promise<void> {
    const backdrop = await this.locateBackdrop();
    if (backdrop) await backdrop.click();
  }
}

