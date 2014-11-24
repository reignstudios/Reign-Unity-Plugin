// -------------------------------------------------------
//  Created by Andrew Witte.
//  Copyright (c) 2013 Reign-Studios. All rights reserved.
// -------------------------------------------------------

#import "StreamNative.h"
#import "UnityTypes.h"

@implementation StreamNative
- (id)init
{
    self = [super init];
    // init other data here...
    return self;
}

- (void)dealloc
{
    // dispose...
    [super dealloc];
}

- (void)image:(UIImage*)image finishedSavingImage:(NSError*)error contextInfo:(void*)contextInfo
{
    if (error) ImageSavedSucceeded = false;
    else ImageSavedSucceeded = true;
    ImageSavedDone = true;
}

- (void)SaveImageToPhotoAlbum:(char*)data dataSize:(int)dataSize
{
    NSData* dataObj = [NSData dataWithBytes:(const void*)data length:dataSize];
    UIImage *image = [[UIImage alloc] initWithData:dataObj];
    if(image)
    {
        ImageSavedSucceeded = false;
        ImageSavedDone = false;
        UIImageWriteToSavedPhotosAlbum(image, self, @selector(image:finishedSavingImage:contextInfo:), NULL);
    }
    else
    {
        ImageSavedSucceeded = false;
        ImageSavedDone = true;
    }
}

- (UIImage*)correctImageRotation:(UIImage*)image :(float)width :(float)height
{
    CGSize size;
    size.width = width;
    size.height = height;
    UIGraphicsBeginImageContext( size );
    [image drawInRect:CGRectMake(0, 0, width, height)];
    UIImage* newImage = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    
    return newImage;
}

- (void)imagePickerController:(UIImagePickerController*)picker didFinishPickingMediaWithInfo:(NSDictionary*)info
{
	// grab the image
	UIImage *image;
	image = [info objectForKey:UIImagePickerControllerOriginalImage];
    
    // rotate image to fit correct pixel width and height
    if (image.imageOrientation == UIImageOrientationLeft)
    {
        image = [self correctImageRotation:image :image.size.width :image.size.height];
    }
    else if (image.imageOrientation == UIImageOrientationRight)
    {
        image = [self correctImageRotation:image :image.size.width :image.size.height];
    }
    else if (image.imageOrientation == UIImageOrientationUp)
    {
        // do nothing
    }
    else if (image.imageOrientation == UIImageOrientationDown)
    {
        image = [self correctImageRotation:image :image.size.width :image.size.height];
    }
    
	// process image
    [self performSelector:@selector(processImageFromImagePicker:) withObject:image];
    
	// dimiss the picker
	[self dismissWrappedController];
}

- (void)dismissWrappedController
{
    //UnityPause(false);
    if (popoverViewController != nil)
    {
        [popoverViewController dismissPopoverAnimated:YES];
        popoverViewController = nil;
    }
    
	UIViewController *vc = UnityGetGLViewController();
	if(vc) [vc dismissModalViewControllerAnimated:YES];
}

- (void)processImageFromImagePicker:(UIImage*)image
{
    NSData *pngData = UIImagePNGRepresentation(image);
    char* tempData = (char*)[pngData bytes];
    imageDataSize = (int)pngData.length;
    imageData = new char[imageDataSize];
    memcpy(imageData, tempData, imageDataSize);
    
    ImageLoadSucceeded = true;
    ImageLoadDone = true;
}

- (void)popoverControllerDidDismissPopover:(UIPopoverController*)popoverController
{
	if (popoverViewController != nil)
    {
        [popoverViewController release];
        popoverViewController = nil;
    }
    //
	//UnityPause(false);
    ImageLoadSucceeded = false;
    ImageLoadDone = true;
}

- (void)imagePickerControllerDidCancel:(UIImagePickerController*)picker
{
	[self dismissWrappedController];
    
	//UnityPause(false);
    ImageLoadSucceeded = false;
    ImageLoadDone = true;
}

- (void)popoverController:(UIPopoverController *)popoverController willRepositionPopoverToRect:(inout CGRect *)rect inView:(inout UIView **)view
{
    // do nothing...
}

- (BOOL)popoverControllerShouldDismissPopover:(UIPopoverController *)popoverController
{
    return YES;
}

- (void)showViewController:(UIViewController*)viewController
{
	//UnityPause(true);
	
	// cancel the previous delayed call to dismiss the view controller if it exists
	[NSObject cancelPreviousPerformRequestsWithTarget:self];
    
    // show the picker
	UIViewController *vc = UnityGetGLViewController();
	[vc presentModalViewController:viewController animated:YES];
}

- (void)ShowPhotoPicker:(UIImagePickerControllerSourceType)type
{
    UIImagePickerController *picker = [[[UIImagePickerController alloc] init] autorelease];
	picker.delegate = self;
	picker.sourceType = type;
	picker.allowsEditing = false;
    
    ImageLoadSucceeded = false;
    ImageLoadDone = false;
    
    // We need to display this in a popover on iPad
    NSString* version = [[UIDevice currentDevice] systemVersion];
	if([version integerValue] < 8.0 && UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad && type != UIImagePickerControllerSourceTypeCamera)
	{
		/*Class popoverClass = NSClassFromString(@"UIPopoverController");
		if(!popoverClass)
        {
            ImageLoadSucceeded = false;
            ImageLoadDone = true;
            return;
        }*/
        
		//popoverViewController = [[popoverClass alloc] initWithContentViewController:picker];
        popoverViewController = [[UIPopoverController alloc] initWithContentViewController:picker];
        [popoverViewController setDelegate:self];
		//picker.modalInPopover = YES;
		
		// Display the popover
        UIWindow* window = [UIApplication sharedApplication].keyWindow;
        [popoverViewController presentPopoverFromRect:PopoverRect inView:window permittedArrowDirections:UIPopoverArrowDirectionAny animated:YES];
	}
	else
    {
        [self showViewController:picker];
    }
}
@end

// ----------------------------------
// Unity C Link
// ----------------------------------
static StreamNative* native = nil;

extern "C"
{
    void InitStream()
    {
        if (native == nil) native = [[StreamNative alloc] init];
    }
    
    void DisposeStream()
    {
        if (native != nil)
        {
            [native release];
            native = nil;
        }
    }
    
    bool CheckImageSavedDoneStatus()
    {
        if (native == nil) return false;
        
        bool done = native->ImageSavedDone;
        native->ImageSavedDone = false;
        return done;
    }
    
    bool CheckImageSavedSucceededStatus()
    {
        return native->ImageSavedSucceeded;
    }
    
    void SaveImageStream(char* data, int dataSize)
    {
        [native SaveImageToPhotoAlbum:data dataSize:dataSize];
    }
    
    bool CheckImageLoadStatus()
    {
        if (native == nil) return false;
        
        bool done = native->ImageLoadDone;
        native->ImageLoadDone = false;
        return done;
    }
    
    bool CheckImageLoadSucceededStatus(char** data, int* dataSize)
    {
        *data = native->imageData;
        *dataSize = native->imageDataSize;
        return native->ImageLoadSucceeded;
    }
    
    void LoadImagePicker(int x, int y, int width, int height)
    {
        native->PopoverRect = CGRectMake(x, y, width, height);
        [native ShowPhotoPicker:UIImagePickerControllerSourceTypePhotoLibrary];
    }
}